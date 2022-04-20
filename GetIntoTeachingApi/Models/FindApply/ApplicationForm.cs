using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GetIntoTeachingApi.Utils;
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

        public Crm.ApplicationForm ToCrmModel()
        {
            var yearId = ((int)Crm.ApplicationForm.RecruitmentCycleYear.Year2020) + (RecruitmentCycleYear - 2020);

            return new Crm.ApplicationForm()
            {
                FindApplyId = Id.ToString(CultureInfo.CurrentCulture),
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                SubmittedAt = SubmittedAt,
                StatusId = (int)Enum.Parse(typeof(Crm.ApplicationForm.Status), ApplicationStatus.ToPascalCase()),
                PhaseId = (int)Enum.Parse(typeof(Crm.ApplicationForm.Phase), ApplicationPhase.ToPascalCase()),
                RecruitmentCycleYearId = yearId,
                Choices = ApplicationChoices?.Select(c => c.ToCrmModel()).ToList(),
                References = References?.Select(c => c.ToCrmModel()).ToList(),
            };
        }
    }
}
