using GetIntoTeachingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Services
{
    public interface ICrmService
    {
        public Task<IEnumerable<TypeEntity>> GetTeachingSubjects();
        public Task<IEnumerable<TypeEntity>> GetCountries();
        public Task<PrivacyPolicy> GetLatestPrivacyPolicy();
    }
}
