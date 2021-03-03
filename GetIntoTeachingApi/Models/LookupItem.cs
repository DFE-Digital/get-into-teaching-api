using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class LookupItem
    {
        public static readonly Guid UnitedKingdomCountryId = new Guid("72f5c2e6-74f9-e811-a97a-000d3a2760f2");
        public static readonly Guid PrimaryTeachingSubjectId = new Guid("b02655a1-2afa-e811-a981-000d3a276620");

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid? Id { get; set; }
        public string Value { get; set; }
        [JsonIgnore]
        public string EntityName { get; set; }

        public LookupItem()
        {
        }

        public LookupItem(Entity entity, string entityName)
        {
            Id = entity.Id;
            Value = entity.GetAttributeValue<string>("dfe_name");
            EntityName = entityName;
        }
    }
}
