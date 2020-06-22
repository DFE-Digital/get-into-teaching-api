using System;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "msevtmgt_eventregistration")]
    public class TeachingEventRegistration : BaseModel
    {
        [EntityField(Name = "msevtmgt_eventid", Type = typeof(EntityReference))]
        public Guid EventId { get; set; }

        public TeachingEventRegistration()
            : base()
        {
        }

        public TeachingEventRegistration(Entity entity, ICrmService crm)
            : base(entity, crm)
        {
        }
    }
}