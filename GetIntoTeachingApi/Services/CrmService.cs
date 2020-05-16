using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

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
            return _service.CreateQuery(ConnectionString(), entityName)
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
            return _service.CreateQuery(ConnectionString(), "dfe_privacypolicy")
                .Where((entity) =>
                    entity.GetAttributeValue<OptionSetValue>("dfe_policytype").Value == (int)PrivacyPolicyType.Web &&
                    entity.GetAttributeValue<bool>("dfe_active")
                )
                .OrderByDescending((policy) => policy.GetAttributeValue<DateTime>("createdon"))
                .Select((entity) => new PrivacyPolicy(entity))
                .Take(MaximumNumberOfPrivacyPolicies);
        }

        public Candidate GetCandidate(ExistingCandidateRequest request)
        {
            return _service.CreateQuery(ConnectionString(), "contact")
                .Where(entity =>
                    // Will perform a case-insensitive comparison
                    entity.GetAttributeValue<string>("emailaddress1") == request.Email
                )
                .OrderByDescending(entity => entity.GetAttributeValue<DateTime>("createdon"))
                .Select(entity => new Candidate(entity))
                .Take(MaximumNumberOfCandidatesToMatch)
                .ToList()
                .FirstOrDefault(request.Match);
        }

        public IEnumerable<CandidateQualification> GetCandidateQualifications(Candidate candidate)
        {
            return _service.CreateQuery(ConnectionString(), "dfe_candidatequalification")
                .Where(entity => entity.GetAttributeValue<Guid>("dfe_contactid") == candidate.Id)
                .Select(entity => new CandidateQualification(entity))
                .ToList();
        }

        public IEnumerable<CandidatePastTeachingPosition> GetCandidatePastTeachingPositions(Candidate candidate)
        {
            return _service.CreateQuery(ConnectionString(), "dfe_candidatepastteachingposition")
                .Where(entity => entity.GetAttributeValue<Guid>("dfe_contactid") == candidate.Id)
                .Select(entity => new CandidatePastTeachingPosition(entity))
                .ToArray();
        }

        public void UpsertCandidate(Candidate candidate)
        {
            using var context = _service.Context(ConnectionString());

            var candidateEntity = UpsertCandidate(candidate, context);

            candidate.Qualifications.ForEach(q => UpsertCandidateQualification(q, candidateEntity, context));
            candidate.PastTeachingPositions.ForEach(p => UpsertCandidatePastTeachingPosition(p, candidateEntity, context));

            CreateCandidatePrivacyPolicy(candidate.PrivacyPolicy, candidateEntity, context);
            CreatePhoneCall(candidate.PhoneCall, candidateEntity, context);

            _service.SaveChanges(context);
        }

        private Entity UpsertCandidate(Candidate candidate, OrganizationServiceContext context)
        {
            var candidateEntity = NewOrExistingEntity(context, "contact", candidate.Id);
            return candidate.ToEntity(candidateEntity);
        }

        private void UpsertCandidateQualification(CandidateQualification qualification, Entity candidateEntity,
            OrganizationServiceContext context)
        {
            var entity = NewOrExistingEntity(context, "dfe_candidatequalification", qualification.Id);
            qualification.ToEntity(entity);

            if (entity.EntityState == EntityState.Created)
                _service.AddLink(candidateEntity,
                    new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), entity, context);
        }

        private void UpsertCandidatePastTeachingPosition(CandidatePastTeachingPosition position, Entity candidateEntity,
            OrganizationServiceContext context)
        {
            var entity = NewOrExistingEntity(context, "dfe_candidatepastteachingposition", position.Id);
            position.ToEntity(entity);

            if (entity.EntityState == EntityState.Created)
                _service.AddLink(candidateEntity,
                    new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), entity, context);
        }

        private void CreateCandidatePrivacyPolicy(CandidatePrivacyPolicy policy, Entity candidateEntity,
            OrganizationServiceContext context)
        {
            if (policy == null) return;

            if (candidateEntity.EntityState == EntityState.Changed)
                if (CandidateAlreadyAcceptedPrivacyPolicy(candidateEntity.Id, policy.AcceptedPolicyId))
                    return;

            var entity = policy.ToEntity(_service.NewEntity("dfe_candidateprivacypolicy", context));
            _service.AddLink(candidateEntity,
                new Relationship("dfe_contact_dfe_candidateprivacypolicy_Candidate"), entity, context);
        }

        private void CreatePhoneCall(PhoneCall phoneCall, Entity candidateEntity, OrganizationServiceContext context)
        {
            if (phoneCall == null) return;

            var entity = _service.NewEntity("phonecall", context);
            phoneCall.ToEntity(entity);
            _service.AddLink(candidateEntity,
                new Relationship("dfe_contact_phonecall_contactid"), entity, context);
        }

        private Entity NewOrExistingEntity(OrganizationServiceContext context, string entityName, Guid? id)
        {
            return id != null ? _service.BlankExistingEntity(entityName, (Guid)id, context) : _service.NewEntity(entityName, context);
        }

        private bool CandidateAlreadyAcceptedPrivacyPolicy(Guid candidateId, Guid privacyPolicyId)
        {
            return _service.CreateQuery(ConnectionString(), "dfe_candidateprivacypolicy")
                .Where(entity => entity.GetAttributeValue<EntityReference>("dfe_candidate").Id == candidateId &&
                                 entity.GetAttributeValue<EntityReference>("dfe_privacypolicynumber").Id ==
                                 privacyPolicyId).FirstOrDefault() != null;
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
