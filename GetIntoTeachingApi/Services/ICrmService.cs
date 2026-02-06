using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeachingApi.Services
{
    public interface ICrmService
    {
        string CheckStatus();
        IEnumerable<Country> GetCountries();
        IEnumerable<TeachingSubject> GetTeachingSubjects();
        IEnumerable<PickListItem> GetPickListItems(string entityName, string attributeName);

        /// <summary>
        /// Retrieves multi-select picklist items for a specified Dataverse entity and attribute,
        /// transforming them into a consistent PickListItem format.
        /// </summary>
        /// <param name="entityName">The logical name of the entity (e.g., "contact")</param>
        /// <param name="attributeName">The logical name of the multi-select attribute</param>
        /// <returns>An IEnumerable of PickListItem objects representing each selectable option</returns>
        IEnumerable<PickListItem> GetMultiSelectPickListItems(
            string entityName, string attributeName);

        IEnumerable<Entity> GetMultiplePickListItems(string entityName, string attributeName);
        IEnumerable<PrivacyPolicy> GetPrivacyPolicies();
        Candidate MatchCandidate(ExistingCandidateRequest request);
        Candidate MatchCandidate(string email, string applyId = null);
        IEnumerable<Candidate> MatchCandidates(string magicLinkToken);
        Candidate GetCandidate(Guid id);
        Candidate GetCandidateWithRelationships(Guid id);
        IEnumerable<Candidate> GetCandidates(IEnumerable<Guid> ids);
        IEnumerable<ContactChannelCreation> GetAllCandidatesContactChannelCreations();
        IEnumerable<Candidate> GetCandidatesPendingMagicLinkTokenGeneration(int limit = 10);
        IEnumerable<TeachingEvent> GetTeachingEvents(DateTime? startAfter = null);
        TeachingEvent GetTeachingEvent(string readableId);
        IEnumerable<CallbackBookingQuota> GetCallbackBookingQuotas();
        CallbackBookingQuota GetCallbackBookingQuota(DateTime scheduledAt);
        IEnumerable<ContactChannelCreation> GetCandidateContactCreations(Guid candidateId);
        public IEnumerable<T> GetApplyModels<T>(IEnumerable<string> applyIds) where T : BaseModel, IHasApplyId;
        bool CandidateAlreadyHasLocalEventSubscriptionType(Guid candidateId);
        bool CandidateYetToAcceptPrivacyPolicy(Guid candidateId, Guid privacyPolicyId);
        bool CandidateYetToRegisterForTeachingEvent(Guid candidateId, Guid teachingEventId);

        bool CandidateHasDegreeQualification(Guid candidateId, CandidateQualification.DegreeType degreeType,
            string degreeSubject);
        void Save(BaseModel model);
        void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
        void DeleteLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
        IEnumerable<Entity> RelatedEntities(Entity entity, string relationshipName, string logicalName);
        Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context);
        Entity NewEntity(string entityName, Guid? id, OrganizationServiceContext context);
        IEnumerable<TeachingEventBuilding> GetTeachingEventBuildings();

        void Execute(OrganizationRequest request);
    }
}