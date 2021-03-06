﻿using System;
using Newtonsoft.Json;

namespace AnyDoDotNet
{
    /// <summary>
    /// "isDefault": true, "id": "fS4A8WlpsZZ8XjmDJ_1Avg==", "default": true, "sharedMembers": null, "lastUpdateDate": 1425888652000, "name": "\u4e2a\u4eba", "isDeleted": false}
    /// </summary>
    public class CategoryInfo
    {
        public bool IsDefault
        {
            get; set;
        }

        public string Id
        {
            get; set;
        }

          [JsonProperty(PropertyName = "default")]
        public bool Default
        {
            get;
            set;
        }

        public string SharedMembers
        {
            get;
            set;
        }

        public DateTime LastUpdateDate
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public bool IsDeleted
        {
            get;
            set;
        }
    }
}
