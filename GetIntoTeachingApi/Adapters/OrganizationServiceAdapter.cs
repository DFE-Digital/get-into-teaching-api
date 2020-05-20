using Microsoft.PowerPlatform.Cds.Client;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using Microsoft.Xrm.Sdk.Query;

namespace GetIntoTeachingApi.Adapters
{
    public class OrganizationServiceAdapter : IOrganizationServiceAdapter
    {
        private readonly IDictionary<string, CdsServiceClient> _clients;

        public OrganizationServiceAdapter()
        {
            _clients = new Dictionary<string, CdsServiceClient>();
        }

        public IQueryable<Entity> CreateQuery(string entityName, OrganizationServiceContext context)
        {
            return context.CreateQuery(entityName);
        }

        public IEnumerable<Entity> RetrieveMultiple(string connectionString, QueryBase query)
        {
            var client = RetrieveClient(connectionString);
            var collection = client.RetrieveMultiple(query);
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
                result.AddRange(entities);

            return result;
        }

        public IEnumerable<CdsServiceClient.PickListItem> GetPickListItemsForAttribute(
            string connectionString,
            string entityName,
            string attributeName
        )
        {
            var client = RetrieveClient(connectionString);
            var metaElement = client.GetPickListElementFromMetadataEntity(entityName, attributeName);
            return metaElement.Items;
        }

        public OrganizationServiceContext Context(string connectionString)
        {
            return ConstructContext(connectionString);
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

        private OrganizationServiceContext ConstructContext(string connectionString)
        {
            return new OrganizationServiceContext(RetrieveClient(connectionString));
        }

        private CdsServiceClient RetrieveClient(string connectionString)
        {
            if (!_clients.ContainsKey(connectionString))
            {
                _clients[connectionString] = new CdsServiceClient(connectionString); 
            }

            return _clients[connectionString];
        }
    }
}
