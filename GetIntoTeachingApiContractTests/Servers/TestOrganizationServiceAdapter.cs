using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Adapters;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetIntoTeachingApiContractTests.Servers
{
    public class TestOrganizationServiceAdapter : IOrganizationServiceAdapter
    {
        private readonly IOrganizationServiceAdapter _client;
        private readonly string _cachePath;

        public TestOrganizationServiceAdapter(IOrganizationService client, string projectPath)
        {
            _client = new OrganizationServiceAdapter(client);
            _cachePath = Path.Combine(projectPath, "../GetIntoTeachingApiContractTests/contracts/");
        }

        private void CacheEntityResponse(string entityName, IQueryable<Entity> response)
        {
            var path = Path.Combine(_cachePath, $"{entityName}_crm.json");
            var data = response.ToArray();
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            
            File.WriteAllText(path, json);
        }

        private void CachePickListItemResponse(string attributeName, IEnumerable<CdsServiceClient.PickListItem> response)
        {
            var path = Path.Combine(_cachePath, $"{attributeName}_crm.json");
            var data = response.ToArray();
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            
            File.WriteAllText(path, json);
        }

        private IQueryable<Entity> GetEntityResponse(string entityName)
        {
            var path = Path.Combine(_cachePath, $"{entityName}_crm.json");
            
            if (!File.Exists(path)) return null;

            return JArray.Parse(File.ReadAllText(path))
                .Select(CreateEntityFromToken)
                .AsQueryable();
        }

        private IEnumerable<CdsServiceClient.PickListItem> GetPickListItemResponse(string attributeName)
        {
            var path = Path.Combine(_cachePath, $"{attributeName}_crm.json");
            
            if (!File.Exists(path)) return null;
            
            return JArray.Parse(File.ReadAllText(path))
                .Select(CreatePickListItemFromToken);
        }

        private static CdsServiceClient.PickListItem CreatePickListItemFromToken(JToken token)
        {
            var key = token["DisplayLabel"]!.ToString();
            var value = token["PickListItemId"]!.Value<int>();

            return new CdsServiceClient.PickListItem(key, value);
        }

        private static Entity CreateEntityFromToken(JToken token)
        {
            var name = token["LogicalName"]!.ToString();
            var guid = Guid.Parse(token["Id"]!.ToString());

            var entity = new Entity(name, guid);
            
            var entityState = token["EntityState"]?.ToObject<int>();
            if (entityState != null)
                entity.EntityState = (EntityState) entityState;

            entity.RowVersion = token["RowVersion"]?.ToString();
            
            token["Attributes"]?.ForEach(attr => {
                    var key = attr["Key"]!.ToString();

                    switch (attr["Value"]?.Type)
                    {
                        case JTokenType.Integer:
                            entity.Attributes.Add(key, attr["Value"].Value<int>());
                            return;
                        case JTokenType.Float:
                            entity.Attributes.Add(key, attr["Value"].Value<float>());
                            return;
                        case JTokenType.String:
                            entity.Attributes.Add(key, attr["Value"].Value<string>());
                            return;
                        case JTokenType.Date:
                            entity.Attributes.Add(key, attr["Value"].Value<DateTime>());
                            return;
                        case JTokenType.Boolean:
                            entity.Attributes.Add(key, attr["Value"].Value<bool>());
                            return;
                    }

                    var value = attr["Value"]!.Value<JObject>();
                    var attrValue = value.Property("Value");
                    
                    if (attrValue == null)
                    {
                        entity.Attributes.Add(key, CreateEntityFromToken(value));
                        return;
                    }

                    entity.Attributes.Add(key, new OptionSetValue((int) attrValue.Value));
                });

            token["FormattedValues"]?.ForEach(attr =>
            {
                var key = attr["Key"]!.ToString();
                var value = attr["Value"]!.ToString();

                entity.FormattedValues.Add(key, value);
            });

            return entity;
        }

        private IQueryable<Entity> CachingEntityFacade(string entityName, Func<IQueryable<Entity>> func)
        {
            var response = GetEntityResponse(entityName);
            
            if ( response != null ) return response;
            
            response = func();
            CacheEntityResponse(entityName, response);

            return response;
        }

        private IEnumerable<CdsServiceClient.PickListItem> CachingPickListItemFacade(string attributeName, Func<IEnumerable<CdsServiceClient.PickListItem>> func)
        {
            var response = GetPickListItemResponse(attributeName);
            
            if ( response != null ) return response;
            
            response = func();
            CachePickListItemResponse(attributeName, response);

            return response;
        }

        public string CheckStatus()
        {
            return _client.CheckStatus();
        }

        public IQueryable<Entity> CreateQuery(string entityName, OrganizationServiceContext context)
        {
            return CachingEntityFacade(entityName, () => _client.CreateQuery(entityName, Context()));
        }

        public IEnumerable<Entity> RetrieveMultiple(QueryBase query)
        {
            switch ((query as QueryExpression)?.EntityName)
            {
                case "msevtmgt_event":
                    return Enumerable.Empty<Entity>();
                default:
                    return _client.RetrieveMultiple(query);
            }
        }

        public void LoadProperty(Entity entity, Relationship relationship, OrganizationServiceContext context)
        {
            _client.LoadProperty(entity, relationship, context);
        }

        public IEnumerable<Entity> RelatedEntities(Entity entity, string attributeName)
        {
            return _client.RelatedEntities(entity, attributeName);
        }

        public IEnumerable<CdsServiceClient.PickListItem> GetPickListItemsForAttribute(
            string entityName,
            string attributeName)
        {
            return CachingPickListItemFacade(attributeName, () => _client.GetPickListItemsForAttribute(entityName, attributeName));
        }

        public OrganizationServiceContext Context()
        {
            return _client.Context();
        }

        public Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context)
        {
            return _client.BlankExistingEntity(entityName, id, context);
        }

        public Entity NewEntity(string entityName, OrganizationServiceContext context)
        {
            return _client.NewEntity(entityName, context);
        }

        public void SaveChanges(OrganizationServiceContext context)
        {
            _client.SaveChanges(context);
        }

        public void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            _client.AddLink(source, relationship, target, context);
        }

        private TaskCompletionSource<bool> _candidateRequestCompleted = new TaskCompletionSource<bool>();
        private object _storedCandidateRequest;
        public async Task<object> GetCandidateRequestDetails()
        {
            await _candidateRequestCompleted.Task;

            return Task.FromResult(_storedCandidateRequest);
        }
    }
}