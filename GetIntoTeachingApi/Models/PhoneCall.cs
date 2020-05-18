using System;
using GetIntoTeachingApi.Attributes;

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
    }
}
