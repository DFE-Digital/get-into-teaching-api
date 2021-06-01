using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace GetIntoTeachingApi.Adapters
{
    public interface IOrganizationServiceAdapter
    {
        string CheckStatus();
        IQueryable<Entity> CreateQuery(string entityName, OrganizationServiceContext context);
        IEnumerable<Entity> RetrieveMultiple(QueryBase query);
        void LoadProperty(Entity entity, Relationship relationship, OrganizationServiceContext context);
        IEnumerable<ServiceClient.PickListItem> GetPickListItemsForAttribute(string entityName, string attributeName);
        IEnumerable<Entity> RelatedEntities(Entity entity, string attributeName);
        OrganizationServiceContext Context();
        Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context);
        Entity NewEntity(string entityName, OrganizationServiceContext context);
        void SaveChanges(OrganizationServiceContext context);
        void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
        void DeleteLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
    }
}
