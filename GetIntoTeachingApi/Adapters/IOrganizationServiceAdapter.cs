using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace GetIntoTeachingApi.Adapters
{
    public interface IOrganizationServiceAdapter
    {
        public IQueryable<Entity> CreateQuery(string entityName, OrganizationServiceContext context);
        public IEnumerable<Entity> RetrieveMultiple(string connectionString, QueryBase query);
        public void LoadProperty(Entity entity, Relationship relationship, OrganizationServiceContext context);
        public IEnumerable<CdsServiceClient.PickListItem> GetPickListItemsForAttribute(string connectionString, string entityName, string attributeName);
        public IEnumerable<Entity> RelatedEntities(Entity entity, string attributeName);
        public OrganizationServiceContext Context(string connectionString);
        public Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context);
        public Entity NewEntity(string entityName, OrganizationServiceContext context);
        public void SaveChanges(OrganizationServiceContext context);
        public void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
    }
}
