using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;

namespace GetIntoTeachingApi.Adapters
{
    public class OrganizationServiceContextAdapter : IOrganizationServiceContextAdapter
    {
        public IQueryable<Entity> CreateQuery(string connectionString, string entityName)
        {
            var client = new CdsServiceClient(connectionString);
            var context = new OrganizationServiceContext(client);
            return context.CreateQuery(entityName);
        }
    }
}
