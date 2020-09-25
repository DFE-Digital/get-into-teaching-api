using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApiContractTests.Builders;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetIntoTeachingApiContractTests.Servers
{
    public class TestOrganizationServiceAdapter : IOrganizationServiceAdapter
    {
        private readonly IOrganizationServiceAdapter _client;
        private readonly string _cachePath;
        private readonly bool _allowPassthroughToCrm;

        public TestOrganizationServiceAdapter(IOrganizationService client, string projectPath, bool allowPassthroughToCrm)
        {
            _client = new OrganizationServiceAdapter(client);
            _cachePath = Path.Combine(projectPath, "../GetIntoTeachingApiContractTests/contracts/");
            _allowPassthroughToCrm = allowPassthroughToCrm;
        }

        private void CacheResponseFor<T>(string name, IEnumerable<T> response)
        {
            var path = Path.Combine(_cachePath, $"{name}_crm.json");
            var data = response.ToArray();
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            
            File.WriteAllText(path, json);
        }

        private IEnumerable<T> GetResponseFor<T>(string entityName, Func<JToken, T> builder)
        {
            var path = Path.Combine(_cachePath, $"{entityName}_crm.json");
            
            if (!File.Exists(path))
                return null;

            return JArray.Parse(File.ReadAllText(path))
                .Select(builder);
        }

        private IQueryable<Entity> CachingEntityFacade(string entityName, Func<IQueryable<Entity>> func)
        {
            var response = GetResponseFor(entityName, CrmEntityBuilder.FromToken);

            if (response == null && _allowPassthroughToCrm)
            {
                CacheResponseFor(entityName, response = func());
            }

            return response?.AsQueryable();
        }

        private IEnumerable<CdsServiceClient.PickListItem> CachingPickListItemFacade(string attributeName, Func<IEnumerable<CdsServiceClient.PickListItem>> func)
        {
            var response = GetResponseFor(attributeName, PickListItemBuilder.FromToken);

            if (response == null && _allowPassthroughToCrm)
            {
                CacheResponseFor(attributeName, response = func());
            }

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
            return (query as QueryExpression)?.EntityName switch
            {
                "msevtmgt_event" => Enumerable.Empty<Entity>(),
                _ => _client.RetrieveMultiple(query)
            };
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

        private OrganizationServiceContext _mockContext;
        public OrganizationServiceContext Context()
        {
            return _allowPassthroughToCrm 
                ? _client.Context()
                : _mockContext ??= new OrganizationServiceContext(new Mock<IOrganizationService>().Object);
        }

        public Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context)
        {
            var blankExistingEntity = _client.BlankExistingEntity(entityName, id, context);
            return blankExistingEntity;
        }

        public Entity NewEntity(string entityName, OrganizationServiceContext context)
        {
            var newEntity = _client.NewEntity(entityName, context);
            _storedCandidateRequests.Add(newEntity);
            return newEntity;
        }

        public void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            _client.AddLink(source, relationship, target, context);
        }

        private bool _candidateRequestCompleted;
        private readonly IList<Entity> _storedCandidateRequests = new List<Entity>();

        public void SaveChanges(OrganizationServiceContext context)
        {
            _candidateRequestCompleted = true;
            
            // Do not actually save to the dynamics CRM
            // _client.SaveChanges(context);
        }

        public Task<Entity> GetCandidateRequests()
        {
            return Task.Run(async () =>
            {
                while (_candidateRequestCompleted == false)
                {
                    await Task.Delay(100);
                }

                var storedContact = _storedCandidateRequests
                    .FirstOrDefault(x => x.LogicalName == "contact");
                _storedCandidateRequests.Clear();
                _candidateRequestCompleted = false;

                return storedContact;
            });
        }
    }
}