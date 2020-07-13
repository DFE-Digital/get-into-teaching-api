using System;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity("dfe_servicesubscription")]
    public class Subscription : BaseModel
    {
        public enum SubscriptionStatus
        {
            Active,
            Inactive,
        }

        public enum ServiceType
        {
            Event = 222750000,
            MailingList = 222750001,
            TeacherTrainingAdviser = 222750002,
        }

        [EntityField("dfe_servicesubscriptiontype", typeof(OptionSetValue))]
        public int? TypeId { get; set; }
        [JsonIgnore]
        [EntityField("statecode", typeof(OptionSetValue))]
        public int StatusId { get; set; } = (int)SubscriptionStatus.Active;
        [EntityField("dfe_servicesubscriptionstartdate")]
        public DateTime StartAt { get; set; } = DateTime.Now;
        [EntityField("dfe_optoutsms")]
        public bool OptOutOfSms { get; set; }
        [EntityField("donotbulkemail")]
        public bool DoNotBulkEmail { get; set; }
        [EntityField("donotbulkpostalmail")]
        public bool DoNotBulkPostalMail { get; set; }
        [EntityField("donotemail")]
        public bool DoNotEmail { get; set; }
        [EntityField("donotpostalmail")]
        public bool DoNotPostalMail { get; set; }
        [EntityField("donotsendmm")]
        public bool DoNotSendMm { get; set; }

        public Subscription()
            : base()
        {
        }

        public Subscription(Entity entity, ICrmService crm)
            : base(entity, crm)
        {
        }
    }
}
