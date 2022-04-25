using System.Linq;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.FindApply
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
                FindApplyId = Id,
                FindApplyCreatedAt = Attributes.CreatedAt,
                FindApplyUpdatedAt = Attributes.UpdatedAt,
                ApplicationForms = Attributes.ApplicationForms?.Select(f => f.ToCrmModel()).ToList(),
            };

            var latestForm = candidate.ApplicationForms?.OrderByDescending(f => f.UpdatedAt).FirstOrDefault();

            if (latestForm == null)
            {
                candidate.FindApplyStatusId = (int)Crm.ApplicationForm.Status.NeverSignedIn;
            }
            else
            {
                candidate.FindApplyStatusId = latestForm.StatusId;
                candidate.FindApplyPhaseId = latestForm.PhaseId;
            }

            return candidate;
        }
    }
}
