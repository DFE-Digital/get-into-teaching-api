using System.Collections.Generic;
using System.Threading.Tasks;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services.Crm
{
    public interface IWebApiClient
    {
        public Task<IEnumerable<TypeEntity>> GetLookupItems(Lookup lookup);
        public Task<IEnumerable<TypeEntity>> GetOptionSetItems(OptionSet optionSet);
    }
}
