using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity("dfe_candidateprivacypolicy")]
    public class CandidatePrivacyPolicy : BaseModel
    {
        public const int Consent = 222750001;

        [EntityField("dfe_privacypolicynumber", typeof(EntityReference), "dfe_privacypolicy")]
        public Guid AcceptedPolicyId { get; set; }
        [EntityField("dfe_consentreceivedby", typeof(OptionSetValue))]
        public int ConsentReceivedById { get; set; } = Consent;
        [EntityField("dfe_meanofconsent", typeof(OptionSetValue))]
        public int MeanOfConsentId { get; set; } = Consent;
        [EntityField("dfe_name")]
        public string Description { get; set; } = "Online consent as part of web registration";
        [EntityField("dfe_timeofconsent")]
        public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;

        public CandidatePrivacyPolicy()
            : base()
        {
        }

        public CandidatePrivacyPolicy(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
            : base(entity, crm, validatorFactory)
        {
        }
    }
}
