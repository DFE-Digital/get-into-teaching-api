using System;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "phonecall")]
    public class PhoneCall : BaseModel
    {
        [EntityField(Name = "dfe_channelcreation", Type = typeof(OptionSetValue))]
        public int? ChannelId { get; set; }
        [EntityField(Name = "scheduledstart")]
        public DateTime ScheduledAt { get; set; }
        [EntityField(Name = "phonenumber")]
        public string Telephone { get; set; }

        public PhoneCall() : base() { }

        public PhoneCall(Entity entity, ICrmService crm) : base(entity, crm) { }
    }
}
