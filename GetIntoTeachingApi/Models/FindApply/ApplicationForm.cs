using System;
using System.Collections.Generic;
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
        [JsonProperty("application_choices.completed")]
        public bool ApplicationChoicesCompleted { get; set; }
        [JsonProperty("application_choices.data")]
        public IEnumerable<ApplicationChoice> ApplicationChoices { get; set; }
        [JsonProperty("references.completed")]
        public bool ReferencesCompleted { get; set; }
        [JsonProperty("references.data")]
        public IEnumerable<Reference> References { get; set; }
        [JsonProperty("qualifications.completed")]
        public bool QualificationsCompleted { get; set; }
        [JsonProperty("personal_statement.completed")]
        public bool PersonalStatementCompleted { get; set; }
    }
}
