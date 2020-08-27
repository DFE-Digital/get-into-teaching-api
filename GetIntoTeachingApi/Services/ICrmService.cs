using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeachingApi.Services
{
    public interface ICrmService
    {
        string CheckStatus();
        IEnumerable<TypeEntity> GetLookupItems(string entityName);
        IEnumerable<TypeEntity> GetPickListItems(string entityName, string attributeName);
        IEnumerable<PrivacyPolicy> GetPrivacyPolicies();
        Candidate MatchCandidate(ExistingCandidateRequest request);
        Candidate GetCandidate(Guid id);
        IEnumerable<TeachingEvent> GetTeachingEvents();
        IEnumerable<CallbackBookingQuota> GetCallbackBookingQuotas();
        bool CandidateYetToAcceptPrivacyPolicy(Guid candidateId, Guid privacyPolicyId);
        bool CandidateYetToRegisterForTeachingEvent(Guid candidateId, Guid teachingEventId);
        void Save(BaseModel model);
        void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
        IEnumerable<Entity> RelatedEntities(Entity entity, string relationshipName, string logicalName);
        Entity MappableEntity(string entityName, Guid? id, OrganizationServiceContext context);
    }
}