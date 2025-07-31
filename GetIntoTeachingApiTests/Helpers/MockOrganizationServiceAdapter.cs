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
    public class MockOrganizationServiceAdapter : IOrganizationServiceAdapter
    {
        public virtual string CheckStatus()
        {
            throw new NotImplementedException();
        }

        public virtual IQueryable<Entity> CreateQuery(string entityName, OrganizationServiceContext context)
        {
            return new List<Entity>().AsQueryable();
        }

        public virtual IEnumerable<Entity> RetrieveMultiple(QueryBase query)
        {
            return new List<Entity>();
        }

        public virtual void LoadProperty(Entity entity, Relationship relationship, OrganizationServiceContext context)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<PickListItem> GetMultiSelectPickListItems(string entityName, string attributeName)
        {
            throw new NotImplementedException();
        }
        
        public virtual IEnumerable<PickListItem> GetPickListItemsForAttribute(string entityName, string attributeName)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<Entity> RelatedEntities(Entity entity, string attributeName)
        {
            throw new NotImplementedException();
        }

        public  OrganizationServiceContext Context()
        {
            return new OrganizationServiceContext(new Mock<IOrganizationService>().Object);
        }

        public virtual Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context)
        {
            return new Entity(entityName, id);
        }

        public virtual Entity NewEntity(string entityName, Guid? id, OrganizationServiceContext context)
        {
            var newEntity = id.HasValue ? new Entity(entityName, id.Value) : new Entity(entityName);
            newEntity.EntityState = EntityState.Created;
            return newEntity;
        }

        public virtual void SaveChanges(OrganizationServiceContext context)
        {
            // Not required.
        }

        public virtual void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            // Not required.
        }

        void IOrganizationServiceAdapter.DeleteLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            throw new NotImplementedException();
        }
    }
}
