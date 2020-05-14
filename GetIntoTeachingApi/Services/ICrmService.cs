using GetIntoTeachingApi.Models;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Services
{
    public interface ICrmService
    {
        public IEnumerable<TypeEntity> GetLookupItems(string entityName);
        public IEnumerable<TypeEntity> GetPickListItems(string entityName, string attributeName);
        public PrivacyPolicy GetLatestPrivacyPolicy();
        public IEnumerable<PrivacyPolicy> GetPrivacyPolicies();
        public Candidate GetCandidate(ExistingCandidateRequest request);
        public IEnumerable<CandidateQualification> GetCandidateQualifications(Candidate candidate);
        public IEnumerable<CandidatePastTeachingPosition> GetCandidatePastTeachingPositions(Candidate candidate);
        public void UpsertCandidate(Candidate candidate);
    }
}
