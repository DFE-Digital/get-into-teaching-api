using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class TypeEntity
    {
        public static readonly Guid UnitedKingdomCountryId = new Guid("72f5c2e6-74f9-e811-a97a-000d3a2760f2");

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string Value { get; set; }
        [JsonIgnore]
        public string EntityName { get; set; }
        [JsonIgnore]
        public string AttributeName { get; set; } = string.Empty;

        public TypeEntity()
        {
        }

        public TypeEntity(Entity entity, string entityName)
        {
            Id = entity.Id.ToString();
            Value = entity.GetAttributeValue<string>("dfe_name");
            EntityName = entityName;
        }

        public TypeEntity(CdsServiceClient.PickListItem pickListItem, string entityName, string attributeName)
        {
            Id = pickListItem.PickListItemId.ToString();
            Value = pickListItem.DisplayLabel;
            EntityName = entityName;
            AttributeName = attributeName;
        }
    }
}
