using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
    public class CandidateAttributes
    {
        [JsonProperty("email_address")]
        public string Email { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [JsonProperty("application_status")]
        public string ApplicationStatus { get; set; }
        [JsonProperty("application_phase")]
        public string ApplicationPhase { get; set; }
        [JsonProperty("application_forms")]
        public IEnumerable<ApplicationForm> ApplicationForms { get; set; }
    }
}
