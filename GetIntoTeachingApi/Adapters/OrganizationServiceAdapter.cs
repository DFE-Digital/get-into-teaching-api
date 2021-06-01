using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Models;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace GetIntoTeachingApi.Adapters
{
    public class OrganizationServiceAdapter : IOrganizationServiceAdapter
    {
        private readonly ServiceClient _client;

        public OrganizationServiceAdapter(IOrganizationService client)
        {
            _client = (ServiceClient)client;
        }

        public string CheckStatus()
        {
            try
            {
                _client.GetMyUserId();
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return HealthCheckResponse.StatusOk;
        }

        public IQueryable<Entity> CreateQuery(string entityName, OrganizationServiceContext context)
        {
            return context.CreateQuery(entityName);
        }

        public IEnumerable<Entity> RetrieveMultiple(QueryBase query)
        {
            var collection = _client.RetrieveMultiple(query);
            return collection.Entities;
        }

        public void LoadProperty(Entity entity, Relationship relationship, OrganizationServiceContext context)
        {
            context.LoadProperty(entity, relationship);
        }

        public IEnumerable<Entity> RelatedEntities(Entity entity, string attributeName)
        {
            var result = new List<Entity>();

            var entities = entity.RelatedEntities
                .Where(pair => pair.Key.SchemaName == attributeName)
                .Select(pair => pair.Value.Entities)
                .FirstOrDefault();

            if (entities != null)
            {
                result.AddRange(entities);
            }

            return result;
        }

        public IEnumerable<ServiceClient.PickListItem> GetPickListItemsForAttribute(
            string entityName,
            string attributeName)
        {
            var metaElement = _client.GetPickListElementFromMetadataEntity(entityName, attributeName);
            return metaElement.Items;
        }

        public OrganizationServiceContext Context()
        {
            return new OrganizationServiceContext(_client);
        }

        public Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context)
        {
            var existingEntity = new Entity(entityName, id);
            context.Attach(existingEntity);
            context.UpdateObject(existingEntity);
            return existingEntity;
        }

        public Entity NewEntity(string entityName, OrganizationServiceContext context)
        {
            var newEntity = new Entity(entityName);
            context.AddObject(newEntity);
            return newEntity;
        }

        public void SaveChanges(OrganizationServiceContext context)
        {
            context.SaveChanges();
        }

        public void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            context.AddLink(source, relationship, target);
        }
    }
}
