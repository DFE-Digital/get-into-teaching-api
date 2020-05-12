using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Collections.Generic;
using System.Linq;

namespace GetIntoTeachingApi.Adapters
{
    public class OrganizationServiceAdapter : IOrganizationServiceAdapter
    {
        private readonly IDictionary<string, CdsServiceClient> _clients;

        public OrganizationServiceAdapter()
        {
            _clients = new Dictionary<string, CdsServiceClient>();
        }

        public IQueryable<Entity> CreateQuery(string connectionString, string entityName)
        {
            var context = Context(connectionString);
            return context.CreateQuery(entityName);
        }

        private OrganizationServiceContext Context(string connectionString)
        {
            return new OrganizationServiceContext(Client(connectionString));
        }

        private CdsServiceClient Client(string connectionString)
        {
            if (!_clients.ContainsKey(connectionString))
            {
                _clients[connectionString] = new CdsServiceClient(connectionString); 
            }

            return _clients[connectionString];
        }
    }
}
