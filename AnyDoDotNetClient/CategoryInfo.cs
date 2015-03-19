using System;
using Newtonsoft.Json;

namespace AnyDoDotNet
{
    /// <summary>
    /// "isDefault": true, "id": "fS4A8WlpsZZ8XjmDJ_1Avg==", "default": true, "sharedMembers": null, "lastUpdateDate": 1425888652000, "name": "\u4e2a\u4eba", "isDeleted": false}
    /// </summary>
    public class CategoryInfo
    {
        public bool isDefault
        {
            get; set;
        }

        public string id
        {
            get; set;
        }

          [JsonProperty(PropertyName = "default")]
        public bool Default
        {
            get;
            set;
        }

        public string sharedMembers
        {
            get;
            set;
        }

        public DateTime lastUpdateDate
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public bool isDeleted
        {
            get;
            set;
        }
    }
}
