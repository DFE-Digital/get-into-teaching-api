using Microsoft.Xrm.Sdk;
using System.Linq;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Adapters
{
    public interface IOrganizationServiceContextAdapter
    {
        public Task<IQueryable<Entity>> CreateQuery(string connectionString, string entityName);
    }
}
