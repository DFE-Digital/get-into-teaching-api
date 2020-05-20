using System;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "msevtmgt_event")]
    public class TeachingEvent : BaseModel
    {
        [EntityField(Name = "dfe_event_type", Type = typeof(OptionSetValue))]
        public int TypeId { get; set; }
        [EntityField(Name = "msevtmgt_name")]
        public string Name { get; set; }
        [EntityField(Name = "msevtmgt_description")]
        public string Description { get; set; }
        [EntityField(Name = "msevtmgt_eventstartdate")]
        public DateTime StartAt { get; set; }
        [EntityField(Name = "msevtmgt_eventenddate")]
        public DateTime EndAt { get; set; }

        public TeachingEvent() : base() { }

        public TeachingEvent(Entity entity, ICrmService crm) : base(entity, crm) { }
    }
}
