﻿using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.Apply
{
    public class ApplicationResponse<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
        [JsonProperty("completed")]
        public bool? Completed { get; set; }
    }
}
