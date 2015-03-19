using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnyDoDotNet
{
   public class TaskCreationInfo : TaskBaseInfo
    {
       public TaskCreationInfo ()
       {
           ListPositionByCategory = "0";
           ListPositionByPriority = "0";
           ListPositionByDueDate = "0";
       }

        public string ListPositionByCategory
        {
            get;
            set;
        }

        public string ListPositionByPriority
        {
            get;
            set;
        }

        public string ListPositionByDueDate
        {
            get;
            set;
        }
    }
}
