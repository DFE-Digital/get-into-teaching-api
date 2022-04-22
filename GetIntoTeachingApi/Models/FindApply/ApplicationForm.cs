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
        [JsonProperty("application_choices")]
        public ApplicationResponse<IEnumerable<ApplicationChoice>> ApplicationChoices { get; set; }
        [JsonProperty("references")]
        public ApplicationResponse<IEnumerable<Reference>> References { get; set; }
        [JsonProperty("qualifications")]
        public ApplicationResponse<IEnumerable<object>> Qualifications { get; set; }
        [JsonProperty("personal_statement")]
        public ApplicationResponse<IEnumerable<object>> PersonalStatement { get; set; }

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
                Choices = ApplicationChoices?.Data?.Select(c => c.ToCrmModel()).ToList(),
                References = References?.Data?.Select(c => c.ToCrmModel()).ToList(),
                ApplicationChoicesCompleted = ApplicationChoices?.Completed,
                ReferencesCompleted = References?.Completed,
                PersonalStatementCompleted = PersonalStatement?.Completed,
                QualificationsCompleted = Qualifications?.Completed,
            };
        }
    }
}
