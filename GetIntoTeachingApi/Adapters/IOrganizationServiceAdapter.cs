using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeachingApi.Adapters
{
    public interface IOrganizationServiceAdapter
    {
        public IQueryable<Entity> CreateQuery(string connectionString, string entityName);
        public IEnumerable<CdsServiceClient.PickListItem> GetPickListItemsForAttribute(string connectionString, string entityName, string attributeName);
        public OrganizationServiceContext Context(string connectionString);
        public Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context);
        public Entity NewEntity(string entityName, OrganizationServiceContext context);
        public void SaveChanges(OrganizationServiceContext context);
        public void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
    }
}
