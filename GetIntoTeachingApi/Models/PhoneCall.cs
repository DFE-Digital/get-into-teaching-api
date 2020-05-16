using System;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class PhoneCall : BaseModel
    {
        [Entity(Name = "scheduledstart")]
        public DateTime ScheduledAt { get; set; }
        [Entity(Name = "phonenumber")]
        public string Telephone { get; set; }

        public PhoneCall() : base() { }

        public PhoneCall(Entity entity) : base(entity) { }
    }
}
