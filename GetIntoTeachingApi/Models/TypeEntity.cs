﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Loggable]
    public class TypeEntity
    {
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
