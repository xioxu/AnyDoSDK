using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnyDoDotNet
{
    /// <summary>
    /// The only required field for create a task is: "Title"
    /// </summary>
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
