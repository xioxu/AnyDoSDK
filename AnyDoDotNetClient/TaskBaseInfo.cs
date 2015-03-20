using System;
using Newtonsoft.Json;

namespace AnyDoDotNet
{
    /// <summary>
    /// {"title":"test","listPositionByCategory":0,"listPositionByPriority":0,"listPositionByDueDate":0,"status":"UNCHECKED","repeatingMethod":"TASK_REPEAT_OFF","shared":false,"priority":"Normal","creationDate":1426489331343,"taskExpanded":false,"categoryId":"fS4A8WlpsZZ8XjmDJ_1Avg==","id":"fdbgoquBXTnuuvImQkA9fg=="}
    /// </summary>
    public class TaskBaseInfo
    {
        public TaskBaseInfo()
        {
            Status = TaskStatus.UNCHECKED;
            RepeatingMethod = TASK_REPEAT.TASK_REPEAT_OFF;
            Priority = TaskPriority.Normal;
        }

        public AlertInfo Alert
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "title")]
        public string Title
        {
            get;
            set;
        }

     

        public TaskStatus Status
        {
            get;
            set;
        }

        public TASK_REPEAT RepeatingMethod
        {
            get;
            set;
        }

        public bool Shared
        {
            get;
            set;
        }

        public string SharedMembers
        {
            get;
            set;
        }

        public TaskPriority Priority
        {
            get;
            set;
        }

        public DateTime CreationDate
        {
            get;
            set;
        }

        public DateTime? DueDate
        {
            get; set;
        }

        public bool TaskExpanded
        {
            get;
            set;
        }

        public string CategoryId
        {
            get;
            set;
        }

        public string Id
        {
            get;
            internal set;
        }
    }
}
