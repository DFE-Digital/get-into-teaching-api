using System;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity("phonecall")]
    public class PhoneCall : BaseModel
    {
        [EntityField("dfe_channelcreation", typeof(OptionSetValue))]
        public int? ChannelId { get; set; }
        [EntityField("scheduledstart")]
        public DateTime ScheduledAt { get; set; }
        [EntityField("phonenumber")]
        public string Telephone { get; set; }

        public PhoneCall()
            : base()
        {
        }

        public PhoneCall(Entity entity, ICrmService crm)
            : base(entity, crm)
        {
        }
    }
}
