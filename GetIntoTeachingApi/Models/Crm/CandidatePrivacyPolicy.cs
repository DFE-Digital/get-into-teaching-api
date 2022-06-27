using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
{
    [SwaggerIgnore]
    [Entity("dfe_candidateprivacypolicy")]
    public class CandidatePrivacyPolicy : BaseModel, IHasCandidateId
    {
        public const int Consent = 222750001;

        [EntityField("dfe_candidate", typeof(EntityReference), "contact")]
        public Guid CandidateId { get; set; }
        [EntityField("dfe_privacypolicynumber", typeof(EntityReference), "dfe_privacypolicy")]
        public Guid AcceptedPolicyId { get; set; }
        [EntityField("dfe_consentreceivedby", typeof(OptionSetValue))]
        public int ConsentReceivedById { get; set; } = Consent;
        [EntityField("dfe_meanofconsent", typeof(OptionSetValue))]
        public int MeanOfConsentId { get; set; } = Consent;
        [EntityField("dfe_name")]
        public string Description { get; set; } = "Online consent as part of web registration";
        [EntityField("dfe_timeofconsent")]
        public DateTime AcceptedAt { get; set; }

        public CandidatePrivacyPolicy()
            : base()
        {
        }

        public CandidatePrivacyPolicy(Entity entity, ICrmService crm, IValidator<CandidatePrivacyPolicy> validator)
            : base(entity, crm, validator)
        {
        }
    }
}
