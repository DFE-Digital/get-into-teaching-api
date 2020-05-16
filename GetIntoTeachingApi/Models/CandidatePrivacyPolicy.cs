using System;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class CandidatePrivacyPolicy : BaseModel
    {
        [Entity(Name = "dfe_privacypolicynumber", Type = typeof(EntityReference), Reference = "dfe_privacypolicy")]
        public Guid AcceptedPolicyId { get; set; }

        public CandidatePrivacyPolicy() : base() { }

        public CandidatePrivacyPolicy(Entity entity) : base(entity) { }
    }
}
