using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AnyDoDotNet
{
    public class AnyDoDotNetClient
    {
        private const string baseUrl = "https://sm-prod2.any.do/";
        private CookieContainer _cookie = new CookieContainer();
        private UserInfo _userInfo = null;
        private CategoryInfo[] _catefories = null;

        public AnyDoDotNetClient()
        {
            initJsonNetConfiguration();
        }

        private void initJsonNetConfiguration()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new UnixDateTimeConverter(),new BoolConverter() }
            };
        }

        /// <summary>
        /// json object, the format like {"name": "Nathan", "creationDate": 1425887868000, "emails": [], "id": "byEd7oQde8ZQ_lInYbK6FQ==", "fake": false, "email": "52280764@qq.com", "phoneNumbers": [], "anonymous": false, "profilePicture": null, "googleAvatarUrl": null}
        /// </summary>
        public UserInfo UserInfo
        {
            get
            {
                return _userInfo;
            }
        }


        private T DoRequest<T>(string url, string requestMethod, dynamic requestData,
            RequestBodyContentType formatType)
        {
            var response = DoRequest(url, requestMethod, requestData, formatType);

            if (string.IsNullOrEmpty(response))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(response,new JsonSerializerSettings() { ContractResolver = new PrivateSetterContractResolver()});
        }

        private string DoRequest(string url, string requestMethod, dynamic requestData, RequestBodyContentType formatType)
        {
            Encoding encoding = Encoding.GetEncoding("utf-8");

            var requestUrl = baseUrl + url;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(new Uri(requestUrl));
                req.CookieContainer = _cookie;
                req.Method = requestMethod;

                if (requestData != null)
                {
                    string requestDataStr = string.Empty;

                    if (formatType == RequestBodyContentType.None)
                    {
                        requestUrl += "?" + convertPostDataToString(requestData);
                        req = (HttpWebRequest)WebRequest.Create(new Uri(requestUrl));
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

                        byte[] bytes = encoding.GetBytes(requestDataStr);

                        req.ContentLength = bytes.Length;

                        Stream stream = req.GetRequestStream();
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Close();
                    }
                }

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string result = sr.ReadToEnd();
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
            foreach (PropertyInfo pi in properties)
            {
                if (str.Length > 0)
                {
                    str.Append("&");
                }

                string strVal = pi.GetValue(postData, null).ToString();

                if (pi.PropertyType == typeof(bool))
                {
                    strVal = strVal.ToLower();
                }
                str.Append(pi.Name + "=").Append(strVal);
            }

            return str.ToString();
        }

        public UserInfo Login(string userName, string pwd)
        {
            var postData = new { j_username = userName, j_password = pwd, _spring_security_remember_me = "on" };
            var response = DoRequest("/j_spring_security_check", "post", postData, RequestBodyContentType.Flatten);

            if (string.IsNullOrEmpty(response))
            {
                _userInfo = DoRequest<UserInfo>("/me", "Get", null, RequestBodyContentType.Flatten);

                GetCategories(false, false);
            }

            return _userInfo;
        }

        /// <summary>
        /// [{"isDefault": true, "id": "fS4A8WlpsZZ8XjmDJ_1Avg==", "default": true, "sharedMembers": null, "lastUpdateDate": 1425888652000, "name": "\u4e2a\u4eba", "isDeleted": false}, {"isDefault": false, "id": "uF9ztE7nUoluDa56T0le_g==", "default": false, "sharedMembers": null, "lastUpdateDate": 1425887872000, "name": "Work", "isDeleted": false}, {"isDefault": false, "id": "RLGLpSRMUivgDln_HjGlfw==", "default": false, "sharedMembers": null, "lastUpdateDate": 1425887872000, "name": "Shopping", "isDeleted": false}, {"isDefault": false, "id": "RLM0r4mnTRCzGlkhfwefOw==", "default": false, "sharedMembers": null, "lastUpdateDate": 1425888651000, "name": "\u5de5\u4f5c", "isDeleted": false}]
        /// </summary>
        public CategoryInfo[] GetCategories(bool includeDeleted, bool includeDone)
        {
            //me/categories?responseType=flat&includeDeleted=false&includeDone=false 
            var result = DoRequest("/me/categories", "get", new { includeDeleted = includeDeleted, includeDone = includeDone }, RequestBodyContentType.None);

            _catefories = JsonConvert.DeserializeObject<CategoryInfo[]>(result);
            return _catefories;
        }

        /// <summary>
        /// Submit the tasks to server
        /// </summary>
        /// <param name="tasks">Json object, sample object:[{"title":"test","listPositionByCategory":0,"listPositionByPriority":0,"listPositionByDueDate":0,"status":"UNCHECKED","repeatingMethod":"TASK_REPEAT_OFF","shared":false,"priority":"Normal","creationDate":1426489331343,"taskExpanded":false,"categoryId":"fS4A8WlpsZZ8XjmDJ_1Avg==","id":"fdbgoquBXTnuuvImQkA9fg=="}]</param>
        public TaskInfo[] SubmitTask(TaskCreationInfo[] tasks,bool enableReminder = false)
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
            return SubmitTask(new TaskCreationInfo[] { task }, enableReminder)[0];
        }

        public TaskInfo[] GetTasks(bool includeDeleted, bool includeDone)
        {
            return DoRequest<TaskInfo[]>("/me/tasks", "get", new { includeDeleted = includeDeleted, includeDone = includeDone }, RequestBodyContentType.None);
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
                if (category.isDefault)
                {
                    return category.id;
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
    }
}
