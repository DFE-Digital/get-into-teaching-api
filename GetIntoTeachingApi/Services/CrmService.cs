using FluentValidation;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using static Dapper.SqlMapper;

namespace GetIntoTeachingApi.Services
{
    public class CrmService : ICrmService
    {
        private const int MaximumNumberOfCandidatesToMatch = 20;
        private const int MaximumNumberOfPrivacyPolicies = 3;
        private const int MaximumCallbackBookingQuotaDaysInAdvance = 14;
        private readonly IOrganizationServiceAdapter _service;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDateTimeProvider _dateTime;
        private readonly IAppSettings _appSettings;
        private readonly ILogger<ICrmService> _logger;
        private readonly TimeSpan _statusCheckInterval = TimeSpan.FromMinutes(1);
        private DateTime _previousStatusCheckAt = DateTime.UtcNow;
        private string _previousStatus;

        public CrmService(
            IOrganizationServiceAdapter service,
            IServiceProvider serviceProvider,
            IAppSettings appSettings,
            IDateTimeProvider dateTime,
            ILogger<ICrmService> logger)
        {
            _appSettings = appSettings;
            _service = service;
            _serviceProvider = serviceProvider;
            _dateTime = dateTime;
            _logger = logger;
        }

        public string CheckStatus()
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                return HealthCheckResponse.StatusIntegrationPaused;
            }

            if (_previousStatus != null && _dateTime.UtcNow.Subtract(_previousStatusCheckAt) < _statusCheckInterval)
            {
                return _previousStatus;
            }

            _previousStatusCheckAt = _dateTime.UtcNow;

            return _previousStatus = _service.CheckStatus();

        }

        public IEnumerable<Country> GetCountries()
        {
            return _service.CreateQuery("dfe_country", Context()).AsEnumerable().Select((entity) => new Country(entity));
        }

        public IEnumerable<TeachingSubject> GetTeachingSubjects()
        {
            return _service.CreateQuery("dfe_teachingsubjectlist", Context()).AsEnumerable().Select((entity) => new TeachingSubject(entity));
        }

        public IEnumerable<PickListItem> GetPickListItems(string entityName, string attributeName)
        {
            return _service.GetPickListItemsForAttribute(entityName, attributeName)
                .Select((pickListItem) => new PickListItem(pickListItem, entityName, attributeName));
        }

        /// <summary>
        /// Retrieves multi-select picklist items for a specified Dataverse entity and attribute,
        /// transforming them into a consistent PickListItem format.
        /// </summary>
        /// <param name="entityName">The logical name of the entity (e.g., "contact")</param>
        /// <param name="attributeName">The logical name of the multi-select attribute</param>
        /// <returns>An IEnumerable of PickListItem objects representing each selectable option</returns>
        public IEnumerable<PickListItem> GetMultiSelectPickListItems(
            string entityName, string attributeName) =>

            // Call the underlying service to get raw picklist items for the specified entity/field
            _service.GetMultiSelectPickListItems(entityName, attributeName)

                // Project (transform) each item into a new PickListItem with additional context
                .Select(pickListItem => new PickListItem(pickListItem, entityName, attributeName));

        public IEnumerable<Entity> GetMultiplePickListItems(string entityName, string attributeName)
        {
            return _service.RetrieveMultiple(new QueryExpression(entityName)
            {
                ColumnSet = new ColumnSet(attributeName),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(attributeName, ConditionOperator.NotNull)
                    }
                }
            });
        }

        public IEnumerable<CallbackBookingQuota> GetCallbackBookingQuotas()
        {
            return _service.CreateQuery("dfe_callbackbookingquota", Context())
                .Where((entity) => entity.GetAttributeValue<DateTime>("dfe_starttime") > _dateTime.UtcNow &&
                                   entity.GetAttributeValue<DateTime>("dfe_starttime") < _dateTime.UtcNow.AddDays(MaximumCallbackBookingQuotaDaysInAdvance))
                .OrderBy((entity) => entity.GetAttributeValue<DateTime>("dfe_starttime"))
                .Select((entity) => new CallbackBookingQuota(entity, this, _serviceProvider))
                .AsEnumerable()
                .Where((quota) => quota.IsAvailable); // Doing this in the Dynamics query throws an exception, though I'm not sure why.
        }

        public CallbackBookingQuota GetCallbackBookingQuota(DateTime scheduledAt)
        {
            return _service.CreateQuery("dfe_callbackbookingquota", Context())
                .Where((entity) => entity.GetAttributeValue<DateTime>("dfe_starttime") == scheduledAt)
                .Select((entity) => new CallbackBookingQuota(entity, this, _serviceProvider))
                .FirstOrDefault();
        }

        public IEnumerable<T> GetApplyModels<T>(IEnumerable<string> applyIds) where T : BaseModel, IHasApplyId
        {
            if (!applyIds.Any())
            {
                return Array.Empty<T>();
            }

            var query = new QueryExpression(BaseModel.LogicalName(typeof(T)));
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(T)));

            var property = typeof(T).GetProperty("ApplyId");
            var attribute = BaseModel.EntityFieldAttribute(property);
            query.Criteria.AddCondition(new ConditionExpression(attribute.Name, ConditionOperator.In, applyIds.ToArray()));

            var entities = _service.RetrieveMultiple(query);


            return entities.Select(e => (T)Activator.CreateInstance(typeof(T), e, this, _serviceProvider));
        }

        public IEnumerable<PrivacyPolicy> GetPrivacyPolicies()
        {
            return _service.CreateQuery("dfe_privacypolicy", Context())
                .Where((entity) =>
                    entity.GetAttributeValue<OptionSetValue>("dfe_policytype").Value == (int)PrivacyPolicy.Type.Web &&
                    entity.GetAttributeValue<bool>("dfe_active"))
                .OrderByDescending((policy) => policy.GetAttributeValue<DateTime>("createdon"))
                .Select((entity) => new PrivacyPolicy(entity, this, _serviceProvider))
                .Take(MaximumNumberOfPrivacyPolicies);
        }

        public Candidate MatchCandidate(ExistingCandidateRequest request)
        {
            var query = MatchBackQuery(request.Email);
            query.TopCount = MaximumNumberOfCandidatesToMatch;

            var entities = _service.RetrieveMultiple(query);
            var entity = entities.FirstOrDefault();

            var status = entity == null ? "Miss" : "Hit";
            _logger.LogInformation("MatchCandidate - EmailMatch - {Status}", status);

            if (entity == null)
            {
                return null;
            }
            
            LoadCandidateRelationships(entity);

            return new Candidate(entity, this, _serviceProvider);
        }

        /// <summary>
        /// Retrieves all <see cref="ContactChannelCreation"/> records linked to a specific candidate.
        /// </summary>
        /// <param name="candidateId">Unique identifier of the candidate.</param>
        /// <returns>
        /// A collection of <see cref="ContactChannelCreation"/> domain models,
        /// constructed from the underlying CRM entities.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="candidateId"/> is an empty GUID.
        /// </exception>
        public IEnumerable<ContactChannelCreation> GetCandidateContactCreations(Guid candidateId)
        {
            if (candidateId == Guid.Empty)
            {
                throw new ArgumentException("CandidateId must not be empty.");
            }

            // Instantiate a CRM query targeting the contact channel creation entity.
            QueryExpression query = new QueryExpression("dfe_contactchannelcreation");

            // Specify the fields to retrieve based on metadata annotations.
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(ContactChannelCreation)));

            // Apply filtering criteria to restrict results to the specified candidate.
            query.Criteria.AddCondition(new ConditionExpression("dfe_contactid", ConditionOperator.Equal, candidateId));

            // Execute the query to fetch matching entities from the CRM service.
            IEnumerable<Entity> entities = _service.RetrieveMultiple(query);

            // Transform raw entities into rich domain models with contextual dependencies.
            return entities.Select(entity =>
                new ContactChannelCreation(entity, this, _serviceProvider));
        }

        public Candidate MatchCandidate(string email, string applyId = null)
        {
            var query = MatchBackQuery(email, applyId);
            query.TopCount = 1;

            var entities = _service.RetrieveMultiple(query);
            var entity = entities.FirstOrDefault();

            var status = entity == null ? "Miss" : "Hit";
            _logger.LogInformation("MatchCandidate - EmailMatch (Apply) - {Status}", status);

            if (entity == null)
            {
                return null;
            }
            
            LoadCandidateRelationships(entity);

            return new Candidate(entity, this, _serviceProvider);
        }

        public IEnumerable<Candidate> MatchCandidates(string magicLinkToken)
        {
            // Avoids a potentially very expensive query.
            if (string.IsNullOrEmpty(magicLinkToken))
            {
                return Array.Empty<Candidate>();
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

            return entities.Select(e => new Candidate(e, this, _serviceProvider));
        }

        public Candidate GetCandidate(Guid id)
        {
            return GetCandidates(new Guid[] { id }).FirstOrDefault();
        }

        public Candidate GetCandidateWithRelationships(Guid id)
        {
            var query = new QueryExpression("contact");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(Candidate)));

            query.Criteria.AddCondition(new ConditionExpression("contactid", ConditionOperator.Equal, id));

            var entity = _service.RetrieveMultiple(query).FirstOrDefault();
            
            LoadCandidateRelationships(entity);
            
            return new Candidate(entity, this, _serviceProvider);
        }

        public IEnumerable<Candidate> GetCandidates(IEnumerable<Guid> ids)
        {
            QueryExpression query = new("contact");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(Candidate)));

            IEnumerable<Entity> entities = _service.RetrieveMultiple(query);

            return entities.Select((entity) => new Candidate(entity, this, _serviceProvider));
        }

        /// <summary>
        /// Retrieves CRM-backed contact channel creation records and maps them to domain models.
        /// Uses a QueryExpression for flexible querying, including relationship-aware mapping.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="ContactChannelCreation"/> instances enriched via CRM entity hydration.
        /// </returns>
        public IEnumerable<ContactChannelCreation> GetAllCandidatesContactChannelCreations()
        {
            // Constructs a CRM query targeting the 'dfe_contactchannelcreation' entity.
            QueryExpression query = new("dfe_contactchannelcreation");
            // Adds columns defined by model metadata, ensuring minimal retrieval footprint.
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(ContactChannelCreation)));
            // Executes the query against CRM using the injected service abstraction.
            IEnumerable<Entity> entities = _service.RetrieveMultiple(query);
            // Maps retrieved entities into strongly typed domain models,
            // injecting this repository context and shared service provider.
            return entities.Select((entity) =>
                new ContactChannelCreation(entity, this, _serviceProvider));
        }

        public IEnumerable<Candidate> GetCandidatesPendingMagicLinkTokenGeneration(int limit = 10)
        {
            var query = new QueryExpression("contact");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(Candidate)));
            query.Criteria.AddCondition(new ConditionExpression("dfe_websitemltokenstatus", ConditionOperator.Equal, (int)Candidate.MagicLinkTokenStatus.Pending));
            query.TopCount = limit;

            var entities = _service.RetrieveMultiple(query);

            return entities.Select(e => new Candidate(e, this, _serviceProvider));
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

        public bool CandidateHasDegreeQualification(Guid candidateId, CandidateQualification.DegreeType degreeType, string degreeSubject)
        {
            // this check helps prevent duplicate qualification records from being created
            return !(_service.CreateQuery("dfe_candidatequalification", Context()).FirstOrDefault(entity =>
                entity.GetAttributeValue<EntityReference>("dfe_contactid").Id == candidateId &&
                entity.GetAttributeValue<int?>("dfe_type") == (int)CandidateQualification.DegreeType.Degree) == null);
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
            var relatedEntityKeys = entity.Attributes.Keys.Where(k => k.StartsWith($"{relationshipName}.", true, CultureInfo.CurrentCulture)).ToList();

            if (relatedEntityKeys.Count == 0)
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

            return entities.Select((entity) => new TeachingEvent(entity, this, _serviceProvider)).ToList();
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

            return new TeachingEvent(entity, this, _serviceProvider);
        }

        public IEnumerable<TeachingEventBuilding> GetTeachingEventBuildings()
        {
            return _service.CreateQuery("msevtmgt_building", Context())
                .Select((entity) => new TeachingEventBuilding(entity, this, _serviceProvider)).ToList();
        }

        private static QueryExpression MatchBackQuery(string email, string applyId = null)
        {
            // The ToList() is important or Dynamics throws an error.
            var emails = EmailReconciler.EquivalentEmails(email).ToList();
            var query = new QueryExpression("contact");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(Candidate)));

            var mainFilter = new FilterExpression(LogicalOperator.And);

            var filter = new FilterExpression(LogicalOperator.Or);
            filter.AddCondition(new ConditionExpression("emailaddress1", ConditionOperator.In, emails));
            filter.AddCondition(new ConditionExpression("emailaddress2", ConditionOperator.In, emails));

            if (applyId != null)
            {
                // We match records on email or apply id.
                filter.AddCondition(new ConditionExpression("dfe_applyid", ConditionOperator.Equal, applyId));

                // Ensure apply id takes presedence over email and duplicate score/modified on.
                query.Orders.Add(new OrderExpression("dfe_applyid", OrderType.Descending));
            }

            mainFilter.AddFilter(filter);

            mainFilter.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, (int)Candidate.Status.Active));

            query.Criteria = mainFilter;

            query.Orders.Add(new OrderExpression("dfe_duplicatescorecalculated", OrderType.Descending));
            query.Orders.Add(new OrderExpression("modifiedon", OrderType.Descending));

            return query;
        }

        private void LoadCandidateRelationships(Entity entity)
        {
            var context = Context();
            context.Attach(entity);
            
            LoadCandidateRelationships(entity, context);
        }
        private void LoadCandidateRelationships(Entity entity, OrganizationServiceContext context)
        {
            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), context);
            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), context);
            _service.LoadProperty(entity, new Relationship("msevtmgt_contact_msevtmgt_eventregistration_Contact"), context);
            
            // TODO: Spencer to confirm no need to call GetContactChanelCreations(candidateId)
        }

        private OrganizationServiceContext Context()
        {
            return _service.Context();
        }
    }
}
