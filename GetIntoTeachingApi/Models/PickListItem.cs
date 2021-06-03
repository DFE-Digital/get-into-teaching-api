using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.PowerPlatform.Cds.Client;

namespace GetIntoTeachingApi.Models
{
    public class PickListItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Value { get; set; }
        [JsonIgnore]
        public string EntityName { get; set; }
        [JsonIgnore]
        public string AttributeName { get; set; } = string.Empty;

        public PickListItem()
        {
        }

        public PickListItem(CdsServiceClient.PickListItem pickListItem, string entityName, string attributeName)
        {
            Id = pickListItem.PickListItemId;
            Value = pickListItem.DisplayLabel;
            EntityName = entityName;
            AttributeName = attributeName;
        }
    }
}
