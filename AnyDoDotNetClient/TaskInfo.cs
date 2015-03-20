using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace AnyDoDotNet
{
    public class TaskInfo : TaskBaseInfo
    {
        public string AssignedTo
        {
            get;
            set;
        }

        public string GlobalTaskId
        {
            get;
            set;
        }

        public DateTime LastUpdateDate
        {
            get;
            set;
        }

        public string Latitude
        {
            get;
            set;
        }

        public string ParentGlobalTaskId
        {
            get;
            set;
        }

        public string[] Participants
        {
            get;
            set;
        }

        public TaskInfo[] SubTasks
        {
            get;
            set;
        }

        public string CategoryCollapsed
        {
            get;
            set;
        }
    }
}
