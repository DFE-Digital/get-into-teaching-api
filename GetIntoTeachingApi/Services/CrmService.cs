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
        private const int MaximumNumberOfCandidatesToMatch = 20;
        private const int MaximumNumberOfPrivacyPolicies = 3;
        private readonly IOrganizationServiceAdapter _service;

        public CrmService(IOrganizationServiceAdapter service)
        {
            _service = service;
        }

        public IEnumerable<TypeEntity> GetLookupItems(string entityName)
        {
            var context = _service.Context(ConnectionString());
            return _service.CreateQuery(entityName, context)
                .Select((entity) => new TypeEntity(entity));
        }

        public IEnumerable<TypeEntity> GetPickListItems(string entityName, string attributeName)
        {
            return _service.GetPickListItemsForAttribute(ConnectionString(), entityName, attributeName)
                .Select((pickListItem) => new TypeEntity(pickListItem));
        }

        public PrivacyPolicy GetLatestPrivacyPolicy()
        {
            return GetPrivacyPolicies().FirstOrDefault();
        }

        public IEnumerable<PrivacyPolicy> GetPrivacyPolicies()
        {
            var context = _service.Context(ConnectionString());
            return _service.CreateQuery("dfe_privacypolicy", context)
                .Where((entity) =>
                    entity.GetAttributeValue<OptionSetValue>("dfe_policytype").Value == (int)PrivacyPolicyType.Web &&
                    entity.GetAttributeValue<bool>("dfe_active")
                )
                .OrderByDescending((policy) => policy.GetAttributeValue<DateTime>("createdon"))
                .Select((entity) => new PrivacyPolicy(entity, _service))
                .Take(MaximumNumberOfPrivacyPolicies);
        }

        public Candidate GetCandidate(ExistingCandidateRequest request)
        {
            var context = _service.Context(ConnectionString());
            var entity = _service.CreateQuery("contact", context)
                .Where(e =>
                    // Will perform a case-insensitive comparison
                    e.GetAttributeValue<string>("emailaddress1") == request.Email
                )
                .OrderByDescending(e => e.GetAttributeValue<DateTime>("createdon"))
                .Take(MaximumNumberOfCandidatesToMatch)
                .ToList()
                .FirstOrDefault(request.Match);

            if (entity == null)
                return null;

            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), context);
            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), context);

            return new Candidate(entity, _service);
        }

        public IEnumerable<CandidateQualification> GetCandidateQualifications(Candidate candidate)
        {
            var context = _service.Context(ConnectionString());
            return _service.CreateQuery("dfe_candidatequalification", context)
                .Where(entity => entity.GetAttributeValue<Guid>("dfe_contactid") == candidate.Id)
                .Select(entity => new CandidateQualification(entity, _service))
                .ToList();
        }

        public IEnumerable<CandidatePastTeachingPosition> GetCandidatePastTeachingPositions(Candidate candidate)
        {
            var context = _service.Context(ConnectionString());
            return _service.CreateQuery("dfe_candidatepastteachingposition", context)
                .Where(entity => entity.GetAttributeValue<Guid>("dfe_contactid") == candidate.Id)
                .Select(entity => new CandidatePastTeachingPosition(entity, _service))
                .ToArray();
        }

        public void UpsertCandidate(Candidate candidate)
        {
            using var context = _service.Context(ConnectionString());
            candidate.ToEntity(_service, context);
            _service.SaveChanges(context);
        }

        private string ConnectionString()
        {
            return $"AuthType=ClientSecret; url={InstanceUrl()}; ClientId={ClientId()}; ClientSecret={ClientSecret()}";
        }

        private static string InstanceUrl()
        {
            return Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
        }

        private static string ClientId()
        {
            return Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
        }

        private static string ClientSecret()
        {
            return Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");
        }
    }
}
