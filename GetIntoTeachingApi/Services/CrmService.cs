using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Services
{
    public class CrmService : ICrmService
    {       
        public enum PrivacyPolicyType { Web = 222750001 }
        private readonly int MaximumNumberOfCandidatesToMatch = 20;
        private readonly IOrganizationServiceAdapter _organizationalService;

        public CrmService(IOrganizationServiceAdapter organizationalService)
        {
            _organizationalService = organizationalService;
        }

        public IEnumerable<TypeEntity> GetLookupItems(string entityName)
        {
            return _organizationalService.CreateQuery(ConnectionString(), entityName)
                .Select((entity) => new TypeEntity(entity));
        }

        public IEnumerable<TypeEntity> GetPickListItems(string entityName, string attributeName)
        {
            return _organizationalService.GetPickListItemsForAttribute(ConnectionString(), entityName, attributeName)
                .Select((pickListItem) => new TypeEntity(pickListItem));
        }

        public PrivacyPolicy GetLatestPrivacyPolicy()
        {
            return _organizationalService.CreateQuery(ConnectionString(), "dfe_privacypolicy")
                .Where((entity) =>
                    entity.GetAttributeValue<OptionSetValue>("dfe_policytype").Value == (int) PrivacyPolicyType.Web &&
                    entity.GetAttributeValue<bool>("dfe_active")
                )
                .OrderByDescending((policy) => policy.GetAttributeValue<DateTime>("createdon"))
                .Select((entity) => new PrivacyPolicy(entity))
                .First();
        }

        public Candidate GetCandidate(ExistingCandidateRequest request)
        {
            var candidate = _organizationalService.CreateQuery(ConnectionString(), "contact")
                .Where(entity =>
                    // Will perform a case-insensitive comparison
                    entity.GetAttributeValue<string>("emailaddress1") == request.Email
                )
                .OrderByDescending(entity => entity.GetAttributeValue<DateTime>("createdon"))
                .Select(entity => new Candidate(entity))
                .Take(MaximumNumberOfCandidatesToMatch)
                .ToList()
                .FirstOrDefault(candidate => request.Match(candidate));

            if (candidate == null) return null;

            candidate.Qualifications = GetCandidateQualifications(candidate);
            candidate.PastTeachingPositions = GetCandidatePastTeachingPositions(candidate);

            return candidate;
        }

        private CandidateQualification[] GetCandidateQualifications(Candidate candidate)
        {
            return _organizationalService.CreateQuery(ConnectionString(), "dfe_candidatequalification")
                .Where(entity => entity.GetAttributeValue<Guid>("dfe_contactid") == candidate.Id)
                .Select(entity => new CandidateQualification(entity))
                .ToArray();
        }

        private CandidatePastTeachingPosition[] GetCandidatePastTeachingPositions(Candidate candidate)
        {
            return _organizationalService.CreateQuery(ConnectionString(), "dfe_candidatepastteachingposition")
                .Where(entity => entity.GetAttributeValue<Guid>("dfe_contactid") == candidate.Id)
                .Select(entity => new CandidatePastTeachingPosition(entity))
                .ToArray();
        }

        private string ConnectionString()
        {
            return $"AuthType=ClientSecret; url={InstanceUrl()}; ClientId={ClientId()}; ClientSecret={ClientSecret()}";
        }

        private string InstanceUrl()
        {
            return Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
        }

        private string ClientId()
        {
            return Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
        }

        private string ClientSecret()
        {
            return Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");
        }
    }
}
