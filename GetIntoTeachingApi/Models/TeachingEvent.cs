using System;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity("msevtmgt_event")]
    public class TeachingEvent : BaseModel
    {
        [EntityField("dfe_event_type", typeof(OptionSetValue))]
        public int TypeId { get; set; }
        [EntityField("msevtmgt_name")]
        public string Name { get; set; }
        [EntityField("msevtmgt_description")]
        public string Description { get; set; }
        [EntityField("msevtmgt_eventstartdate")]
        public DateTime StartAt { get; set; }
        [EntityField("msevtmgt_eventenddate")]
        public DateTime EndAt { get; set; }
        [EntityRelationship("msevtmgt_event_building", typeof(TeachingEventBuilding))]
        public TeachingEventBuilding Building { get; set; }

        public TeachingEvent()
            : base()
        {
        }

        public TeachingEvent(Entity entity, ICrmService crm)
            : base(entity, crm)
        {
        }
    }
}
