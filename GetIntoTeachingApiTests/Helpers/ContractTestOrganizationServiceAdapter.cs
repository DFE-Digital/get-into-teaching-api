using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Adapters;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Moq;

namespace GetIntoTeachingApiTests.Helpers
{
    public class ContractTestOrganizationServiceAdapter : IOrganizationServiceAdapter
    {
        public readonly List<Entity> TrackedEntities = new List<Entity>();

        public string CheckStatus()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Entity> CreateQuery(string entityName, OrganizationServiceContext context)
        {
            return new List<Entity>().AsQueryable();
        }

        public IEnumerable<Entity> RetrieveMultiple(QueryBase query)
        {
            return new List<Entity>();
        }

        public void LoadProperty(Entity entity, Relationship relationship, OrganizationServiceContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PickListItem> GetPickListItemsForAttribute(string entityName, string attributeName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entity> RelatedEntities(Entity entity, string attributeName)
        {
            throw new NotImplementedException();
        }

        public OrganizationServiceContext Context()
        {
            return new OrganizationServiceContext(new Mock<IOrganizationService>().Object);
        }

        public Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context)
        {
            var existingEntity = new Entity(entityName, id);
            TrackedEntities.Add(existingEntity);
            return existingEntity;
        }

        public Entity NewEntity(string entityName, Guid? id, OrganizationServiceContext context)
        {
            var newEntity = id.HasValue ? new Entity(entityName, id.Value) : new Entity(entityName);
            newEntity.EntityState = EntityState.Created;
            TrackedEntities.Add(newEntity);
            return newEntity;
        }

        public void SaveChanges(OrganizationServiceContext context)
        {
            // Not required (exposed via TrackedEntities).
        }

        public void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            // Related entities are nested against an already-tracked entity,
            // so they don't need to be tracked explicitly.
            source.Attributes[relationship.SchemaName] = target;
            TrackedEntities.Remove(target);
        }

        void IOrganizationServiceAdapter.DeleteLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            throw new NotImplementedException();
        }
    }
}
