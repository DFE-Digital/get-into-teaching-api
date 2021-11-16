using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ICrmService> _logger;

        public CrmService(
            IOrganizationServiceAdapter service,
            IValidatorFactory validatorFactory,
            IAppSettings appSettings,
            IDateTimeProvider dateTime,
            ILogger<ICrmService> logger)
        {
            _appSettings = appSettings;
            _service = service;
            _validatorFactory = validatorFactory;
            _dateTime = dateTime;
            _logger = logger;
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

        public ApplicationForm GetApplicationForm(string findApplyId)
        {
            var query = new QueryExpression("dfe_applyapplicationform");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(ApplicationForm)));
            query.Criteria.AddCondition(new ConditionExpression("dfe_applicationformid", ConditionOperator.Equal, findApplyId));

            var entities = _service.RetrieveMultiple(query);
            var entity = entities.FirstOrDefault();

            if (entity == null)
            {
                return null;
            }

            return new ApplicationForm(entity, this, _validatorFactory);
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
            var query = MatchBackQuery(request.Email);
            query.TopCount = MaximumNumberOfCandidatesToMatch;

            var entities = _service.RetrieveMultiple(query);
            var entity = entities.FirstOrDefault(request.IsFullMatch);

            if (entity == null)
            {
                entity = entities.FirstOrDefault(request.IsEmailMatch);

                _logger.LogInformation($"MatchCandidate - EmailMatch - {(entity == null ? "Miss" : "Hit")}");
            }

            if (entity == null)
            {
                return null;
            }

            var context = Context();
            context.Attach(entity);

            LoadCandidateRelationships(entity, context);

            return new Candidate(entity, this, _validatorFactory);
        }

        public Candidate MatchCandidate(string email)
        {
            var query = MatchBackQuery(email);
            query.TopCount = 1;

            var entities = _service.RetrieveMultiple(query);
            var entity = entities.FirstOrDefault();

            if (entity == null)
            {
                return null;
            }

            return new Candidate(entity, this, _validatorFactory);
        }

        public IEnumerable<Candidate> MatchCandidates(string magicLinkToken)
        {
            // Avoids a potentially very expensive query.
            if (string.IsNullOrEmpty(magicLinkToken))
            {
                return new Candidate[0];
            }

            var query = new QueryExpression("contact");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(Candidate)));
            query.Criteria.AddCondition(new ConditionExpression("dfe_websitemltoken", ConditionOperator.Equal, magicLinkToken));

            var entities = _service.RetrieveMultiple(query);
            var context = Context();

            foreach (var entity in entities)
            {
                context.Attach(entity);
                LoadCandidateRelationships(entity, context);
            }

            return entities.Select(e => new Candidate(e, this, _validatorFactory));
        }

        public Candidate GetCandidate(Guid id)
        {
            return GetCandidates(new Guid[] { id }).FirstOrDefault();
        }

        public IEnumerable<Candidate> GetCandidates(IEnumerable<Guid> ids)
        {
            if (!ids.Any())
            {
                return Array.Empty<Candidate>();
            }

            var query = new QueryExpression("contact");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(Candidate)));

            query.Criteria.AddCondition(new ConditionExpression("contactid", ConditionOperator.In, ids.ToArray()));

            var entities = _service.RetrieveMultiple(query);

            return entities.Select((entity) => new Candidate(entity, this, _validatorFactory));
        }

        public IEnumerable<Candidate> GetCandidatesPendingMagicLinkTokenGeneration(int limit = 10)
        {
            var query = new QueryExpression("contact");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(Candidate)));
            query.Criteria.AddCondition(new ConditionExpression("dfe_websitemltokenstatus", ConditionOperator.Equal, (int)Candidate.MagicLinkTokenStatus.Pending));
            query.TopCount = limit;

            var entities = _service.RetrieveMultiple(query);

            return entities.Select(e => new Candidate(e, this, _validatorFactory));
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

        public Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context)
        {
            return _service.BlankExistingEntity(entityName, id, context);
        }

        public Entity NewEntity(string entityName, Guid? id, OrganizationServiceContext context)
        {
            return _service.NewEntity(entityName, id, context);
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
            var query = new QueryExpression("msevtmgt_event");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(TeachingEvent)));
            var readableIdCondition = new ConditionExpression("dfe_websiteeventpartialurl", ConditionOperator.Equal, readableId);
            query.Criteria.AddCondition(readableIdCondition);

            var entity = _service.RetrieveMultiple(query).FirstOrDefault();

            if (entity == null)
            {
                return null;
            }

            var context = Context();
            context.Attach(entity);
            _service.LoadProperty(entity, new Relationship("msevtmgt_event_building"), context);

            return new TeachingEvent(entity, this, _validatorFactory);
        }

        public IEnumerable<TeachingEventBuilding> GetTeachingEventBuildings()
        {
            return _service.CreateQuery("msevtmgt_building", Context())
                .Select((entity) => new TeachingEventBuilding(entity, this, _validatorFactory)).ToList();
        }

        private static QueryExpression MatchBackQuery(string email)
        {
            var query = new QueryExpression("contact");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(Candidate)));
            query.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, (int)Candidate.Status.Active));
            query.Criteria.AddCondition(new ConditionExpression("emailaddress1", ConditionOperator.Equal, email));
            query.Orders.Add(new OrderExpression("dfe_duplicatescorecalculated", OrderType.Descending));
            query.Orders.Add(new OrderExpression("modifiedon", OrderType.Descending));

            return query;
        }

        private void LoadCandidateRelationships(Entity entity, OrganizationServiceContext context)
        {
            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), context);
            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), context);
            _service.LoadProperty(entity, new Relationship("msevtmgt_contact_msevtmgt_eventregistration_Contact"), context);
        }

        private OrganizationServiceContext Context()
        {
            return _service.Context();
        }
    }
}
