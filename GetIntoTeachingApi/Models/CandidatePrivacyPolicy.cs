using System;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class CandidatePrivacyPolicy
    {
        public Guid AcceptedPolicyId { get; set; }

        public Entity PopulateEntity(Entity entity)
        {
            entity["dfe_privacypolicynumber"] = new EntityReference("dfe_privacypolicy", AcceptedPolicyId);

            return entity;
        }
    }
}
