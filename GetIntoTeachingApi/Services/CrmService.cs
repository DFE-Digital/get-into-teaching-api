using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace GetIntoTeachingApi.Services
{
    public class CrmService : ICrmService
    {
        private const int MaximumNumberOfCandidatesToMatch = 20;
        private const int MaximumNumberOfPrivacyPolicies = 3;
        private const int MaximumCallbackBookingQuotaDaysInAdvance = 14;
        private readonly IOrganizationServiceAdapter _service;
        private readonly IValidatorFactory _validatorFactory;
        private readonly IDateTimeProvider _dateTime;
        private readonly IAppSettings _appSettings;

        public CrmService(
            IOrganizationServiceAdapter service,
            IValidatorFactory validatorFactory,
            IAppSettings appSettings,
            IDateTimeProvider dateTime)
        {
            _appSettings = appSettings;
            _service = service;
            _validatorFactory = validatorFactory;
            _dateTime = dateTime;
        }

        public string CheckStatus()
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                return HealthCheckResponse.StatusIntegrationPaused;
            }

            return _service.CheckStatus();
        }

        public IEnumerable<LookupItem> GetLookupItems(string entityName)
        {
            return _service.CreateQuery(entityName, Context()).Select((entity) => new LookupItem(entity, entityName));
        }

        public IEnumerable<PickListItem> GetPickListItems(string entityName, string attributeName)
        {
            return _service.GetPickListItemsForAttribute(entityName, attributeName)
                .Select((pickListItem) => new PickListItem(pickListItem, entityName, attributeName));
        }

        public IEnumerable<CallbackBookingQuota> GetCallbackBookingQuotas()
        {
            return _service.CreateQuery("dfe_callbackbookingquota", Context())
                .Where((entity) => entity.GetAttributeValue<DateTime>("dfe_starttime") > _dateTime.UtcNow &&
                                   entity.GetAttributeValue<DateTime>("dfe_starttime") < _dateTime.UtcNow.AddDays(MaximumCallbackBookingQuotaDaysInAdvance))
                .OrderBy((entity) => entity.GetAttributeValue<DateTime>("dfe_starttime"))
                .Select((entity) => new CallbackBookingQuota(entity, this, _validatorFactory))
                .ToList()
                .Where((quota) => quota.IsAvailable); // Doing this in the Dynamics query throws an exception, though I'm not sure why.
        }

        public CallbackBookingQuota GetCallbackBookingQuota(DateTime scheduledAt)
        {
            return _service.CreateQuery("dfe_callbackbookingquota", Context())
                .Where((entity) => entity.GetAttributeValue<DateTime>("dfe_starttime") == scheduledAt)
                .Select((entity) => new CallbackBookingQuota(entity, this, _validatorFactory))
                .FirstOrDefault();
        }

        public IEnumerable<PrivacyPolicy> GetPrivacyPolicies()
        {
            return _service.CreateQuery("dfe_privacypolicy", Context())
                .Where((entity) =>
                    entity.GetAttributeValue<OptionSetValue>("dfe_policytype").Value == (int)PrivacyPolicy.Type.Web &&
                    entity.GetAttributeValue<bool>("dfe_active"))
                .OrderByDescending((policy) => policy.GetAttributeValue<DateTime>("createdon"))
                .Select((entity) => new PrivacyPolicy(entity, this, _validatorFactory))
                .Take(MaximumNumberOfPrivacyPolicies);
        }

        public Candidate MatchCandidate(ExistingCandidateRequest request)
        {
            var context = Context();
            var entity = _service.CreateQuery("contact", context)
                .Where(e =>
                    e.GetAttributeValue<int>("statecode") == (int)Candidate.Status.Active &&

                    // Will perform a case-insensitive comparison.
                    // Contains is used to ensure we match emails with white space (request.Match does an exact match in-memory).
                    e.GetAttributeValue<string>("emailaddress1").Contains(request.Email))
                .OrderByDescending(e => e.GetAttributeValue<double>("dfe_duplicatescorecalculated"))
                .ThenByDescending(e => e.GetAttributeValue<DateTime>("modifiedon"))
                .Take(MaximumNumberOfCandidatesToMatch)
                .FirstOrDefault(request.Match);

            if (entity == null)
            {
                return null;
            }

            LoadCandidateRelationships(entity, context);

            return new Candidate(entity, this, _validatorFactory);
        }

        public IEnumerable<Candidate> MatchCandidates(string magicLinkToken)
        {
            // Avoids a potentially very expensive query.
            if (string.IsNullOrEmpty(magicLinkToken))
            {
                return new Candidate[0];
            }

            var context = Context();
            var entities = _service.CreateQuery("contact", context)
                .Where(c => c.GetAttributeValue<string>("dfe_websitemltoken") == magicLinkToken);

            foreach (var entity in entities)
            {
                LoadCandidateRelationships(entity, context);
            }

            return entities.Select(e => new Candidate(e, this, _validatorFactory));
        }

        public Candidate GetCandidate(Guid id)
        {
            var entity = _service.CreateQuery("contact", Context())
                .FirstOrDefault(c => c.GetAttributeValue<EntityReference>("contactid") != null &&
                c.GetAttributeValue<EntityReference>("contactid").Id == id);

            return entity == null ? null : new Candidate(entity, this, _validatorFactory);
        }

        public IEnumerable<Candidate> GetCandidatesPendingMagicLinkTokenGeneration(int limit = 10)
        {
            return _service.CreateQuery("contact", Context())
                .Where(entity => entity.GetAttributeValue<OptionSetValue>("dfe_websitemltokenstatus") != null &&
                    entity.GetAttributeValue<OptionSetValue>("dfe_websitemltokenstatus").Value == (int)Candidate.MagicLinkTokenStatus.Pending)
                .Take(limit)
                .Select(e => new Candidate(e, this, _validatorFactory));
        }

        public bool CandidateAlreadyHasLocalEventSubscriptionType(Guid candidateId)
        {
            return GetCandidate(candidateId).EventsSubscriptionTypeId == (int)Candidate.SubscriptionType.LocalEvent;
        }

        public bool CandidateYetToAcceptPrivacyPolicy(Guid candidateId, Guid privacyPolicyId)
        {
            return _service.CreateQuery("dfe_candidateprivacypolicy", Context()).FirstOrDefault(entity =>
                entity.GetAttributeValue<EntityReference>("dfe_candidate").Id == candidateId &&
                entity.GetAttributeValue<EntityReference>("dfe_privacypolicynumber").Id == privacyPolicyId) == null;
        }

        public bool CandidateYetToRegisterForTeachingEvent(Guid candidateId, Guid teachingEventId)
        {
            return _service.CreateQuery("msevtmgt_eventregistration", Context()).FirstOrDefault(entity =>
                entity.GetAttributeValue<EntityReference>("msevtmgt_contactid").Id == candidateId &&
                entity.GetAttributeValue<EntityReference>("msevtmgt_eventid").Id == teachingEventId) == null;
        }

        public void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            _service.AddLink(source, relationship, target, context);
        }

        public void DeleteLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            _service.DeleteLink(source, relationship, target, context);
        }

        public IEnumerable<Entity> RelatedEntities(Entity entity, string relationshipName, string logicalName)
        {
            var relatedEntityKeys = entity.Attributes.Keys.Where(k => k.StartsWith($"{relationshipName}.")).ToList();

            if (!relatedEntityKeys.Any())
            {
                // If we used LINQ and AddProperty the related entities are already in the context
                // and can be queried with the relationship.
                return _service.RelatedEntities(entity, relationshipName);
            }

            // If we used a QueryExpression and AddLink the related entities are left outer joined
            // into the parent entity, keyed under the relationship name.
            var id = entity.GetAttributeValue<AliasedValue>($"{relationshipName}.{logicalName}id").Value;
            var relatedEntity = new Entity() { Id = (Guid)id };

            foreach (var key in relatedEntityKeys)
            {
                relatedEntity.Attributes[key.Replace($"{relationshipName}.", string.Empty)] =
                    entity.GetAttributeValue<AliasedValue>(key).Value;
            }

            return new List<Entity>() { relatedEntity };
        }

        public Entity MappableEntity(string entityName, Guid? id, OrganizationServiceContext context)
        {
            return id != null ? _service.BlankExistingEntity(entityName, (Guid)id, context) : _service.NewEntity(entityName, context);
        }

        public void Save(BaseModel model)
        {
            using var context = Context();
            var entity = model.ToEntity(this, context);

            if (entity == null)
            {
                return;
            }

            _service.SaveChanges(context);
            model.Id = entity.Id;
        }

        public IEnumerable<TeachingEvent> GetTeachingEvents(DateTime? startAfter = null)
        {
            if (startAfter == null)
            {
                startAfter = _dateTime.UtcNow;
            }

            var query = new QueryExpression("msevtmgt_event");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(TeachingEvent)));

            var status = new[] { (int)TeachingEvent.Status.Open, (int)TeachingEvent.Status.Closed, (int)TeachingEvent.Status.Pending };
            var statusCondition = new ConditionExpression("dfe_eventstatus", ConditionOperator.In, status);
            var futureDatedCondition = new ConditionExpression("msevtmgt_eventenddate", ConditionOperator.GreaterThan, startAfter);
            var types = Enum.GetValues(typeof(TeachingEvent.EventType)).Cast<int>().ToArray();
            var typeCondition = new ConditionExpression("dfe_event_type", ConditionOperator.In, types);
            var readableIdCondition = new ConditionExpression("dfe_websiteeventpartialurl", ConditionOperator.NotNull);
            var filter = new FilterExpression(LogicalOperator.And);
            filter.Conditions.AddRange(new[] { statusCondition, futureDatedCondition, typeCondition, readableIdCondition });
            query.Criteria.AddFilter(filter);

            var entities = _service.RetrieveMultiple(query);

            return entities.Select((entity) => new TeachingEvent(entity, this, _validatorFactory)).ToList();
        }

        public TeachingEvent GetTeachingEvent(string readableId)
        {
            return _service.CreateQuery("msevtmgt_event", Context())
                .Where(entity => entity.GetAttributeValue<string>("dfe_websiteeventpartialurl") == readableId)
                .Select(entity => new TeachingEvent(entity, this, _validatorFactory))
                .SingleOrDefault();
        }

        public IEnumerable<TeachingEventBuilding> GetTeachingEventBuildings()
        {
            return _service.CreateQuery("msevtmgt_building", Context())
                .Select((entity) => new TeachingEventBuilding(entity, this, _validatorFactory)).ToList();
        }

        private void LoadCandidateRelationships(Entity entity, OrganizationServiceContext context)
        {
            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), context);
            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), context);
            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_servicesubscription_contact"), context);
            _service.LoadProperty(entity, new Relationship("msevtmgt_contact_msevtmgt_eventregistration_Contact"), context);
        }

        private OrganizationServiceContext Context()
        {
            return _service.Context();
        }
    }
}
