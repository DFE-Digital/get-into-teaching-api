using System.Collections.Generic;
using System.Threading.Tasks;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services.Crm
{
    public interface IWebApiClient
    {
        Task<IEnumerable<TypeEntity>> GetLookupItems(Lookup lookup);
        Task<IEnumerable<TypeEntity>> GetOptionSetItems(OptionSet optionSet);
        Task<PrivacyPolicy> GetLatestPrivacyPolicy();
        Task<IEnumerable<PrivacyPolicy>> GetPrivacyPolicies();
        Task<Candidate> GetCandidate(ExistingCandidateRequest request);
    }
}
