using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

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
            GetIntoTeaching = 222750012,
        }

        [EntityField("dfe_event_type", typeof(OptionSetValue))]
        public int TypeId { get; set; }
        [EntityField("dfe_eventstatus", typeof(OptionSetValue))]
        public int StatusId { get; set; }
        [EntityField("dfe_eventregion", typeof(OptionSetValue))]
        public int? RegionId { get; set; }
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

        /// <summary>
        /// Represents the raw accessibility options <see cref="OptionSetValueCollection"> for the Accessibility options derived from the CRM.
        /// </summary>
        [JsonIgnore]
        [NotMapped] // Prevent EF Core from mapping this
        [EntityField("dfe_accessibility", typeof(OptionSetValueCollection), ignore: true)]
        public OptionSetValueCollection AccessibilityOptionSet { get; set; }

        /// <summary>
        /// Exposes a comma-separated string of accessibility options.
        /// Getter returns selected option values as a comma-separated string.
        /// Setter parses a comma-separated string into an OptionSetValueCollection.
        /// </summary>
        [JsonIgnore]
        public string AccessibilityOptionId // Named according to the CRM field (required).
        {
            get
            {
                // If the OptionSet collection is not null, project each OptionSetValue to its integer value as a string
                // Join the strings with commas, using invariant culture for consistent formatting.
                return AccessibilityOptionSet != null
                    ? string.Join(",", AccessibilityOptionSet.Select(opt =>
                        opt.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                    : null; // If null, return null
            }
            set
            {
                // If the input string is not null or whitespace, split it into parts
                // Parse each string into an integer, wrap it in OptionSetValue, and collect into a collection
                // Assign the collection back to the AccessibilityOptionSet.
                AccessibilityOptionSet = !string.IsNullOrWhiteSpace(value)
                    ? new OptionSetValueCollection(
                        value.Split(',')
                             .Select(s => new OptionSetValue(
                                 int.Parse(s.Trim(), System.Globalization.CultureInfo.InvariantCulture)))
                             .ToList())
                    : null; // If input is null or empty, clear the OptionSet
            }
        }

        /// <summary>
        /// Provides an array of integers representing accessibility option IDs,
        /// parsed from or serialized to a comma-separated string.
        /// This property is not mapped to the database directly.
        /// </summary>
        [NotMapped]
        public int[] AccessibilityOptions
        {
            get => ParseAccessibilityOptions(AccessibilityOptionId);                // Converts the raw string into an int array
            set => AccessibilityOptionId = SerializeAccessibilityOptions(value);    // Converts int array back to CSV string
        }

        /// <summary>
        /// Parses a comma-separated string into an array of integers.
        /// Returns an empty array if the input is null or whitespace.
        /// If a segment fails to parse, it is assigned the fallback value -1.
        /// </summary>
        /// <param name="rawValue">The string containing comma-separated numbers.</param>
        /// <returns>An array of integers parsed from the string.</returns>
        private static int[] ParseAccessibilityOptions(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return Array.Empty<int>(); // Return empty if no data provided

            return rawValue
                .Split(',') // Split by commas
                .Select(s => int.TryParse(s.Trim(), out int n) ? n : -1) // Parse each entry; fallback to -1 on failure
                .ToArray(); // Convert to array
        }

        /// <summary>
        /// Converts an array of integers into a comma-separated string.
        /// </summary>
        /// <param name="options">The integer array to serialize.</param>
        /// <returns>A comma-separated string representing the values.</returns>
        private static string SerializeAccessibilityOptions(int[] options)
        {
            return string.Join(",", options); // Combine into CSV format
        }

        // The department refers to 'virtual' events as "in-person" (as
        // well as offline events), so whilst virtual events are in fact online,
        // they are deemed in-person here for consistency.
        public bool IsInPerson => !IsOnline || IsVirtual;

        public TeachingEvent()
            : base()
        {
        }

        public TeachingEvent(Entity entity, ICrmService crm, IServiceProvider serviceProvider)
            : base(entity, crm, serviceProvider)
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
