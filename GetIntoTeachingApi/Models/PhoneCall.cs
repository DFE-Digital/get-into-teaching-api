using System;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "phonecall")]
    public class PhoneCall : BaseModel
    {
        [EntityField(Name = "scheduledstart")]
        public DateTime ScheduledAt { get; set; }
        [EntityField(Name = "phonenumber")]
        public string Telephone { get; set; }

        public PhoneCall() : base() { }

        public PhoneCall(Entity entity, IOrganizationServiceAdapter service) : base(entity, service) { }
    }
}
