using System;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "dfe_candidateprivacypolicy")]
    public class CandidatePrivacyPolicy : BaseModel
    {
        public const int Consent = 222750001;

        [EntityField(Name = "dfe_privacypolicynumber", Type = typeof(EntityReference), Reference = "dfe_privacypolicy")]
        public Guid AcceptedPolicyId { get; set; }
        [JsonIgnore]
        [EntityField(Name = "dfe_consentreceivedby", Type = typeof(OptionSetValue))]
        public int ConsentReceivedById { get; set; } = Consent;
        [JsonIgnore]
        [EntityField(Name = "dfe_meanofconsent", Type = typeof(OptionSetValue))]
        public int MeanOfConsentId { get; set; } = Consent;
        [JsonIgnore]
        [EntityField(Name = "dfe_name")]
        public string Description { get; set; } = "Online consent as part of web registration";
        [JsonIgnore]
        [EntityField(Name = "dfe_timeofconsent")]
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
