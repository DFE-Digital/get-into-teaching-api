using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models.Crm
{
    [Entity("msevtmgt_event")]
    public class TeachingEvent : BaseModel
    {
        private const int GmtTimeZoneCode = 85;

        private string _name;

        public enum Status
        {
            Open = 222750000,
            Closed = 222750001,
            Draft = 222750002,
            Pending = 222750003,
        }

        public enum EventType
        {
            ApplicationWorkshop = 222750000,
            TrainToTeachEvent = 222750001,
            OnlineEvent = 222750008,
            SchoolOrUniversityEvent = 222750009,
            QuestionTime = 222750007,
        }

        [EntityField("dfe_event_type", typeof(OptionSetValue))]
        public int TypeId { get; set; }
        [EntityField("dfe_eventstatus", typeof(OptionSetValue))]
        public int StatusId { get; set; }
        [EntityField("dfe_websiteeventpartialurl")]
        public string ReadableId { get; set; }
        [EntityField("dfe_eventwebfeedid")]
        [SwaggerSchema("If set, the API will accept new attendees for " +
            "this event (an external sign up should be used if this value is nil).")]
        public string WebFeedId { get; set; }
        [EntityField("dfe_isonlineevent")]
        public bool IsOnline { get; set; }
        [EntityField("dfe_externaleventtitle")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                InternalName = value;
            }
        }

        [NotMapped]
        [JsonIgnore]
        [EntityField("msevtmgt_name")]
        public string InternalName { get; set; }
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
        [EntityField("dfe_scribbleurl")]
        public string ScribbleId { get; set; }
        [EntityField("dfe_providerwebsite")]
        public string ProviderWebsiteUrl { get; set; }
        [EntityField("dfe_providertargetaudience_ml")]
        public string ProviderTargetAudience { get; set; }
        [EntityField("dfe_providerorganiser")]
        public string ProviderOrganiser { get; set; }
        [EntityField("dfe_providercontactemailaddress")]
        public string ProviderContactEmail { get; set; }
        [NotMapped]
        [JsonIgnore]
        [EntityField("msevtmgt_eventtimezone")]
        public int InternalTimeZone { get; set; } = GmtTimeZoneCode;
        [EntityField("msevtmgt_eventstartdate")]
        public DateTime StartAt { get; set; }
        [EntityField("msevtmgt_eventenddate")]
        public DateTime EndAt { get; set; }
        [EntityField("dfe_providerslist")]
        public string ProvidersList { get; set; }
        [EntityRelationship("msevtmgt_event_building", typeof(TeachingEventBuilding))]
        public TeachingEventBuilding Building { get; set; }
        [JsonIgnore]
        [EntityField("msevtmgt_building", typeof(EntityReference), "msevtmgt_building")]
        public Guid? BuildingId { get; set; }
        public bool IsVirtual => IsOnline && !string.IsNullOrWhiteSpace(Building?.AddressPostcode);

        // The department refers to 'virtual' events as "in-person" (as
        // well as offline events), so whilst virtual events are in fact online,
        // they are deemed in-person here for consistency.
        public bool IsInPerson => !IsOnline || IsVirtual;

        public TeachingEvent()
            : base()
        {
        }

        public TeachingEvent(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
            : base(entity, crm, validatorFactory)
        {
        }

        protected override void FinaliseEntity(Entity source, ICrmService crm, OrganizationServiceContext context)
        {
            var existingEvent = crm.GetTeachingEvent(ReadableId);

            bool removeBuilding = Building == null && existingEvent?.Building != null;

            if (removeBuilding)
            {
                DeleteLink(source, crm, context, existingEvent.Building, nameof(existingEvent.Building));
            }
        }
    }
}
