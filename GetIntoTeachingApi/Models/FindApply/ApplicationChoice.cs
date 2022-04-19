using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
    public class ApplicationChoice
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("provider")]
        public Provider Provider { get; set; }
        [JsonProperty("course")]
        public Provider Course { get; set; }
        [JsonProperty("interviews")]
        public IEnumerable<Interview> Interviews { get; set; }
    }
}
