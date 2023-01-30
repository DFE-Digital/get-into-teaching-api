using System.Linq;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.Apply
{
    public class Candidate
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("attributes")]
        public CandidateAttributes Attributes { get; set; }

        public Crm.Candidate ToCrmModel()
        {
            var candidate = new Crm.Candidate()
            {
                Email = Attributes.Email,
                ApplyId = Id,
                ApplyCreatedAt = Attributes.CreatedAt,
                ApplyUpdatedAt = Attributes.UpdatedAt,
                ApplicationForms = Attributes.ApplicationForms?.Select(f => f.ToCrmModel()).ToList(),
            };

            var latestForm = candidate.ApplicationForms?.OrderByDescending(f => f.UpdatedAt).FirstOrDefault();

            if (latestForm == null)
            {
                candidate.ApplyStatusId = (int)Crm.ApplicationForm.Status.NeverSignedIn;
            }
            else
            {
                candidate.ApplyStatusId = latestForm.StatusId;
                candidate.ApplyPhaseId = latestForm.PhaseId;
            }

            return candidate;
        }
    }
}
