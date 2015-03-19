using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AnyDoDotNet
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TaskStatus
    {
        DELETED,
        UNCHECKED,
        CHECKED,
        DONE
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TASK_REPEAT
    {
        TASK_REPEAT_OFF,
        TASK_REPEAT_DAY,
        TASK_REPEAT_WEEK,
        TASK_REPEAT_MONTH
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TaskPriority
    {
        High,
        Normal
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TASK_REPEAT_END_Type
    {
        REPEAT_END_NEVER
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Repeat_Monty_Type
    {
        ON_DATE
    }
}
