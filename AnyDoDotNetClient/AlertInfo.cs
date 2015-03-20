using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace AnyDoDotNet
{
    public class AlertInfo
    {
        public string CustomeTime
        {
            get;
            set;
        }

        public string OffSet
        {
            get;
            set;
        }

        public string RepeatDays
        {
            get;
            set;
        }

        public int? RepeatEndsAfterOccurrences
        {
            get;
            set;
        }

        public TASK_REPEAT_END_Type? RepeatEndType
        {
            get;
            set;
        }

        public int? RepeatInterval
        {
            get;
            set;
        }

        public Repeat_Monty_Type? RepeatMonthType
        {
            get;
            set;
        }

        public string RepeatNextOccurrence
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "type")]
        public string AlertType
        {
            get;
            set;
        }

        public string RepeatStartsOn
        {
            get;
            set;
        }
    }
}
