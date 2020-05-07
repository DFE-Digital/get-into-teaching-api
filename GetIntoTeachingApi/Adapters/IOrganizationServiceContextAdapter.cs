using Microsoft.Xrm.Sdk;
using System.Linq;

namespace GetIntoTeachingApi.Adapters
{
    public interface IOrganizationServiceContextAdapter
    {
        public IQueryable<Entity> CreateQuery(string connectionString, string entityName);
    }
}
