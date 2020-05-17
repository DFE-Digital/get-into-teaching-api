using System;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "dfe_candidateprivacypolicy")]
    public class CandidatePrivacyPolicy : BaseModel
    {
        [EntityField(Name = "dfe_privacypolicynumber", Type = typeof(EntityReference), Reference = "dfe_privacypolicy")]
        public Guid AcceptedPolicyId { get; set; }

        public CandidatePrivacyPolicy() : base() { }

        public CandidatePrivacyPolicy(Entity entity, IOrganizationServiceAdapter service) : base(entity, service) { }
    }
}
