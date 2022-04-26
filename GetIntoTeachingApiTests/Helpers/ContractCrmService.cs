using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Newtonsoft.Json;

namespace GetIntoTeachingApiTests.Helpers
{
    public class ContractCrmService : ICrmService
    {
        private readonly ICrmService _crmService;
        private IEnumerable<Candidate> Candidates
        {
            get
            {
                var json = File.ReadAllText("./Contracts/Data/candidates.json");
                return JsonConvert.DeserializeObject<IEnumerable<Candidate>>(json);
            }
        }
        private IEnumerable<ApplicationForm> ApplicationForms
        {
            get
            {
                var json = File.ReadAllText("./Contracts/Data/application_forms.json");
                return JsonConvert.DeserializeObject<IEnumerable<ApplicationForm>>(json);
            }
        }

        public ContractCrmService(ICrmService crmService)
        {
            _crmService = crmService;
        }

        public void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            _crmService.AddLink(source, relationship, target, context);
        }

        public Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context)
        {
            return _crmService.BlankExistingEntity(entityName, id, context);
        }

        public bool CandidateAlreadyHasLocalEventSubscriptionType(Guid candidateId)
        {
            return _crmService.CandidateAlreadyHasLocalEventSubscriptionType(candidateId);
        }

        public bool CandidateYetToAcceptPrivacyPolicy(Guid candidateId, Guid privacyPolicyId)
        {
            return _crmService.CandidateYetToAcceptPrivacyPolicy(candidateId, privacyPolicyId);
        }

        public bool CandidateYetToRegisterForTeachingEvent(Guid candidateId, Guid teachingEventId)
        {
            return _crmService.CandidateYetToRegisterForTeachingEvent(candidateId, teachingEventId);
        }

        public string CheckStatus()
        {
            return _crmService.CheckStatus();
        }

        public void DeleteLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            _crmService.DeleteLink(source, relationship, target, context);
        }

        public IEnumerable<T> GetFindApplyModels<T>(IEnumerable<string> findApplyIds) where T : BaseModel, IHasFindApplyId
        {
            switch (typeof(T).ToString())
            {
                case "GetIntoTeachingApi.Models.Crm.ApplicationForm":
                    return ApplicationForms.Where(f => findApplyIds.Contains(f.FindApplyId)) as IEnumerable<T>;
                default:
                    throw new NotImplementedException();
            }
        }

        public CallbackBookingQuota GetCallbackBookingQuota(DateTime scheduledAt)
        {
            return _crmService.GetCallbackBookingQuota(scheduledAt);
        }

        public IEnumerable<CallbackBookingQuota> GetCallbackBookingQuotas()
        {
            return _crmService.GetCallbackBookingQuotas();
        }

        public Candidate GetCandidate(Guid id)
        {
            return Candidates.FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<Candidate> GetCandidates(IEnumerable<Guid> ids)
        {
            return Candidates.Where(c => ids.Contains((Guid)c.Id));
        }

        public IEnumerable<Candidate> GetCandidatesPendingMagicLinkTokenGeneration(int limit = 10)
        {
            return _crmService.GetCandidatesPendingMagicLinkTokenGeneration(limit);
        }

        public IEnumerable<LookupItem> GetLookupItems(string entityName)
        {
            return _crmService.GetLookupItems(entityName);
        }

        public IEnumerable<PickListItem> GetPickListItems(string entityName, string attributeName)
        {
            return _crmService.GetPickListItems(entityName, attributeName);
        }

        public IEnumerable<PrivacyPolicy> GetPrivacyPolicies()
        {
            return _crmService.GetPrivacyPolicies();
        }

        public TeachingEvent GetTeachingEvent(string readableId)
        {
            return _crmService.GetTeachingEvent(readableId);
        }

        public IEnumerable<TeachingEventBuilding> GetTeachingEventBuildings()
        {
            return _crmService.GetTeachingEventBuildings();
        }

        public IEnumerable<TeachingEvent> GetTeachingEvents(DateTime? startAfter = null)
        {
            return _crmService.GetTeachingEvents(startAfter);
        }

        public Candidate MatchCandidate(ExistingCandidateRequest request)
        {
            return _crmService.MatchCandidate(request);
        }

        public Candidate MatchCandidate(string email)
        {
            return Candidates.FirstOrDefault(c => c.Email == email);
        }

        public IEnumerable<Candidate> MatchCandidates(string magicLinkToken)
        {
            return _crmService.MatchCandidates(magicLinkToken);
        }

        public Entity NewEntity(string entityName, Guid? id, OrganizationServiceContext context)
        {
            return _crmService.NewEntity(entityName, id, context);
        }

        public IEnumerable<Entity> RelatedEntities(Entity entity, string relationshipName, string logicalName)
        {
            return _crmService.RelatedEntities(entity, relationshipName, logicalName);
        }

        public void Save(BaseModel model)
        {
            _crmService.Save(model);
        }
    }
}
