using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AnyDoDotNet
{
    public class AnyDoDotNetClient
    {
        private const string baseUrl = "https://sm-prod2.any.do/";
        private readonly CookieContainer _cookie = new CookieContainer();
        private CategoryInfo[] _catefories;
        
        public delegate void RequestDone();
        public delegate void ResponseDone();
        public event RequestDone RequestDoneEvent;
        public event ResponseDone ResponseDoneEvent;

        public AnyDoDotNetClient()
        {
            UserInfo = null;
            initJsonNetConfiguration();
        }

        public UserInfo UserInfo { get; private set; }

        public UserInfo Login(string userName, string pwd)
        {
            var postData = new {j_username = userName, j_password = pwd, _spring_security_remember_me = "on"};
            var response = DoRequest("/j_spring_security_check", "post", postData, RequestBodyContentType.Flatten);

            if (string.IsNullOrEmpty(response))
            {
                UserInfo = DoRequest<UserInfo>("/me", "Get", null, RequestBodyContentType.Flatten);

                GetCategories(false, false);
            }

            return UserInfo;
        }

        public CategoryInfo[] GetCategories(bool includeDeleted = false, bool includeDone = false, bool refresh = false)
        {
            if (!refresh && _catefories != null)
            {
                return _catefories;
            }

            //me/categories?responseType=flat&includeDeleted=false&includeDone=false 
            var result = DoRequest("/me/categories", "get", new {includeDeleted, includeDone},
                RequestBodyContentType.None);

            _catefories = JsonConvert.DeserializeObject<CategoryInfo[]>(result);
            return _catefories;
        }

        public TaskInfo[] SubmitTask(TaskCreationInfo[] tasks, bool enableReminder = false)
        {
            if (tasks == null)
            {
                return null;
            }

            tasks.ToList().ForEach(x =>
                                   {
                                       x.Id = Util.CreateGlobalId();
                                       x.CreationDate = DateTime.Now;
                                       if (string.IsNullOrEmpty(x.CategoryId))
                                       {
                                           x.CategoryId = getDefaultCategoryId();
                                       }

                                       if (x.Alert == null && enableReminder)
                                       {
                                           x.Alert = createDefaultReminder();
                                       }
                                   });


            return DoRequest<TaskInfo[]>("/me/tasks", "post", tasks, RequestBodyContentType.Json);
        }

        public TaskInfo SubmitTask(TaskCreationInfo task, bool enableReminder = false)
        {
            return SubmitTask(new[] {task}, enableReminder)[0];
        }

        public TaskInfo[] GetTasks(bool includeDeleted, bool includeDone)
        {
            return DoRequest<TaskInfo[]>("/me/tasks", "get", new {includeDeleted, includeDone},
                RequestBodyContentType.None);
        }

        public TaskInfo GetTask(string taskId)
        {
            return DoRequest<TaskInfo>("/me/tasks/" + taskId, "get", null, RequestBodyContentType.None);
        }

        public TaskInfo UpdateTask(TaskInfo task)
        {
            if (task == null)
            {
                return null;
            }

            return DoRequest<TaskInfo>("/me/tasks/" + task.Id, "put", task, RequestBodyContentType.Json);
        }

        private string getDefaultCategoryId()
        {
            foreach (var category in _catefories)
            {
                if (category.IsDefault)
                {
                    return category.Id;
                }
            }

            return null;
        }

        private AlertInfo createDefaultReminder()
        {
            var alertInfo = new AlertInfo();
            alertInfo.OffSet = "0";
            alertInfo.RepeatDays = "0000000";
            alertInfo.RepeatEndType = TASK_REPEAT_END_Type.REPEAT_END_NEVER;
            alertInfo.RepeatEndsAfterOccurrences = -1;
            alertInfo.RepeatInterval = 1;
            alertInfo.RepeatMonthType = Repeat_Monty_Type.ON_DATE;
            alertInfo.AlertType = "OFFSET";

            return alertInfo;
        }

        private void initJsonNetConfiguration()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                                                {
                                                    Formatting = Formatting.Indented,
                                                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                                    Converters =
                                                        new List<JsonConverter>
                                                        {
                                                            new UnixDateTimeConverter()
                                                        }
                                                };
        }

        private T DoRequest<T>(string url, string requestMethod, dynamic requestData,
            RequestBodyContentType formatType)
        {
            var response = DoRequest(url, requestMethod, requestData, formatType);

            if (string.IsNullOrEmpty(response))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(response,
                new JsonSerializerSettings {ContractResolver = new PrivateSetterContractResolver()});
        }

        private string DoRequest(string url, string requestMethod, dynamic requestData,
            RequestBodyContentType formatType)
        {
            var encoding = Encoding.GetEncoding("utf-8");

            var requestUrl = baseUrl + url;
            try
            {
                var req = (HttpWebRequest) WebRequest.Create(new Uri(requestUrl));
                req.CookieContainer = _cookie;
                req.Method = requestMethod;

                if (requestData != null)
                {
                    var requestDataStr = string.Empty;

                    if (formatType == RequestBodyContentType.None)
                    {
                        requestUrl += "?" + convertPostDataToString(requestData);
                        req = (HttpWebRequest) WebRequest.Create(new Uri(requestUrl));
                        req.CookieContainer = _cookie;
                        req.Method = requestMethod;
                    }
                    else
                    {
                        if (formatType == RequestBodyContentType.Json)
                        {
                            requestDataStr = JsonConvert.SerializeObject(requestData);
                            req.ContentType = "application/json";
                        }
                        else if (formatType == RequestBodyContentType.Flatten)
                        {
                            requestDataStr = convertPostDataToString(requestData);
                            req.ContentType = "application/x-www-form-urlencoded";
                        }

                        var bytes = encoding.GetBytes(requestDataStr);

                        req.ContentLength = bytes.Length;

                        var stream = req.GetRequestStream();
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Close();

                        if (RequestDoneEvent != null)
                        {
                            RequestDoneEvent();
                        }
                    }
                }

                var response = (HttpWebResponse) req.GetResponse();
                var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                var result = sr.ReadToEnd();

                if (ResponseDoneEvent != null)
                {
                    ResponseDoneEvent();
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private string convertPostDataToString(dynamic postData)
        {
            if (postData == null)
            {
                return string.Empty;
            }

            var str = new StringBuilder();

            PropertyInfo[] properties = postData.GetType().GetProperties();
            foreach (var pi in properties)
            {
                if (str.Length > 0)
                {
                    str.Append("&");
                }

                string strVal = pi.GetValue(postData, null).ToString();

                if (pi.PropertyType == typeof (bool))
                {
                    strVal = strVal.ToLower();
                }
                str.Append(pi.Name + "=").Append(strVal);
            }

            return str.ToString();
        }
    }
}