using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class LookupItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid? Id { get; set; }
        public string Value { get; set; }
        [JsonIgnore]
        public string EntityName { get; set; }

        public LookupItem()
        {
        }

        public LookupItem(Entity entity)
        {
            Id = entity.Id;
            Value = entity.GetAttributeValue<string>("dfe_name");
            EntityName = entity.LogicalName;
        }
    }
}
