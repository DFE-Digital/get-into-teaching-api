using Microsoft.Xrm.Sdk;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.PowerPlatform.Cds.Client.CdsServiceClient;

namespace GetIntoTeachingApi.Adapters
{
    public interface IOrganizationServiceAdapter
    {
        public IQueryable<Entity> CreateQuery(string connectionString, string entityName);
        public IEnumerable<PickListItem> GetPickListItemsForAttribute(string connectionString, string entityName, string attributeName);
    }
}
