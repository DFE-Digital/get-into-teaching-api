using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeachingApi.Services
{
    public interface ICrmService
    {
        IEnumerable<TypeEntity> GetLookupItems(string entityName);
        IEnumerable<TypeEntity> GetPickListItems(string entityName, string attributeName);
        IEnumerable<PrivacyPolicy> GetPrivacyPolicies();
        Candidate GetCandidate(ExistingCandidateRequest request);
        IEnumerable<TeachingEvent> GetTeachingEvents();
        bool CandidateYetToAcceptPrivacyPolicy(Guid candidateId, Guid privacyPolicyId); 
        bool CandidateYetToRegisterForTeachingEvent(Guid candidateId, Guid teachingEventId);
        void Save(BaseModel model);
        void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
        IEnumerable<Entity> RelatedEntities(Entity entity, string attributeName, string logicalName);
        Entity MappableEntity(string entityName, Guid? id, OrganizationServiceContext context);
    }
}
