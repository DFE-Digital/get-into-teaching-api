using System;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    [Entity("msevtmgt_event")]
    public class TeachingEvent : BaseModel
    {
        public enum Status
        {
            Open = 222750000,
            Closed = 222750001,
            Draft = 222750002,
        }

        public enum EventType
        {
            ApplicationWorkshop = 222750000,
            TrainToTeachEvent = 222750001,
            OnlineEvent = 222750008,
            SchoolOrUniversityEvent = 222750009,
        }

        [EntityField("dfe_event_type", typeof(OptionSetValue))]
        public int TypeId { get; set; }
        [EntityField("dfe_eventstatus", typeof(OptionSetValue))]
        public int StatusId { get; set; }
        [EntityField("msevtmgt_readableeventid")]
        public string ReadableId { get; set; }
        [EntityField("dfe_eventwebfeedid")]
        [SwaggerSchema("If set, the API will accept new attendees for " +
            "this event (an external sign up should be used if this value is nil).")]
        public string WebFeedId { get; set; }
        [EntityField("dfe_isonlineevent")]
        public bool IsOnline { get; set; }
        [EntityField("msevtmgt_name")]
        public string Name { get; set; }
        [EntityField("dfe_externaleventtitle")]
        public string ExternalName { get; set; }
        [EntityField("dfe_eventsummary_ml")]
        public string Summary { get; set; }
        [EntityField("dfe_miscellaneousmessage_ml")]
        [SwaggerSchema("Used to push miscellaneous messages to users " +
            "(if an event is close to being booked out, for example).")]
        public string Message { get; set; }
        [EntityField("msevtmgt_description")]
        public string Description { get; set; }
        [EntityField("dfe_videolink")]
        public string VideoUrl { get; set; }
        [EntityField("dfe_providerwebsite")]
        public string ProviderWebsiteUrl { get; set; }
        [EntityField("dfe_providertargetaudience_ml")]
        public string ProviderTargetAudience { get; set; }
        [EntityField("dfe_providerorganiser")]
        public string ProviderOrganiser { get; set; }
        [EntityField("dfe_providercontactemailaddress")]
        public string ProviderContactEmail { get; set; }
        [EntityField("msevtmgt_eventstartdate")]
        public DateTime StartAt { get; set; }
        [EntityField("msevtmgt_eventenddate")]
        public DateTime EndAt { get; set; }
        [EntityRelationship("msevtmgt_event_building", typeof(TeachingEventBuilding))]
        public TeachingEventBuilding Building { get; set; }
        [JsonIgnore]
        public Guid? BuildingId { get; set; }

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
