using System;
using System.Text.Json.Serialization;
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
        [JsonIgnore]
        [EntityField("dfe_consentreceivedby", typeof(OptionSetValue))]
        public int ConsentReceivedById { get; set; } = Consent;
        [JsonIgnore]
        [EntityField("dfe_meanofconsent", typeof(OptionSetValue))]
        public int MeanOfConsentId { get; set; } = Consent;
        [JsonIgnore]
        [EntityField("dfe_name")]
        public string Description { get; set; } = "Online consent as part of web registration";
        [JsonIgnore]
        [EntityField("dfe_timeofconsent")]
        public DateTime AcceptedAt { get; set; } = DateTime.Now;

        public CandidatePrivacyPolicy()
            : base()
        {
        }

        public CandidatePrivacyPolicy(Entity entity, ICrmService crm)
            : base(entity, crm)
        {
        }
    }
}
