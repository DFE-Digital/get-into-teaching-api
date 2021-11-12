using System;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
{
    public class ApplicationForm
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonProperty("submitted_at")]
        public DateTime? SubmittedAt { get; set; }
        [JsonProperty("application_status")]
        public string ApplicationStatus { get; set; }
        [JsonProperty("application_phase")]
        public string ApplicationPhase { get; set; }
        [JsonProperty("recruitment_cycle_year")]
        public int RecruitmentCycleYear { get; set; }
    }
}
