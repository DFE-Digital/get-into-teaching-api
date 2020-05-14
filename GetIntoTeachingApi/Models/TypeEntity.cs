using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class TypeEntity
    {
        public dynamic Id { get; set; }
        public dynamic Value { get; set; }

        public TypeEntity() {}

        public TypeEntity(Entity entity)
        {
            Id = entity.Id;
            Value = entity.GetAttributeValue<string>("dfe_name");
        }

        public TypeEntity(CdsServiceClient.PickListItem pickListItem)
        {
            Id = pickListItem.PickListItemId;
            Value = pickListItem.DisplayLabel;
        }
    }
}
