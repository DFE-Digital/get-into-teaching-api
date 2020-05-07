using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GetIntoTeachingApi.Adapters
{
    public class OrganizationServiceContextAdapter : IOrganizationServiceContextAdapter
    {
        private readonly IDictionary<string, OrganizationServiceContext> _contexts;

        public OrganizationServiceContextAdapter()
        {
            _contexts = new Dictionary<string, OrganizationServiceContext>();
        }

        public IQueryable<Entity> CreateQuery(string connectionString, string entityName)
        {
            return Context(connectionString).CreateQuery(entityName);
        }

        private OrganizationServiceContext Context(string connectionString)
        {
            if (!_contexts.ContainsKey(connectionString))
            {
                var client = new CdsServiceClient(connectionString);
                _contexts[connectionString] = new OrganizationServiceContext(client);
            }

            return _contexts[connectionString];
        }
    }
}
