using System;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class PhoneCall
    {
        public DateTime ScheduledAt { get; set; }

        public Entity PopulateEntity(Entity entity, string telephone)
        {
            entity["phonenumber"] = telephone;
            entity["scheduledstart"] = ScheduledAt;

            return entity;
        }
    }
}
