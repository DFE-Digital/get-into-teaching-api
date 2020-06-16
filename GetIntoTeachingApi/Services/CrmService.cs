﻿using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace GetIntoTeachingApi.Services
{
    public class CrmService : ICrmService
    {
        public enum PrivacyPolicyType
        {
            Web = 222750001,
        }

        private readonly IOrganizationServiceAdapter _service;
        private const int MaximumNumberOfCandidatesToMatch = 20;
        private const int MaximumNumberOfPrivacyPolicies = 3;

        public CrmService(IOrganizationServiceAdapter service)
        {
            _service = service;
        }

        public IEnumerable<TypeEntity> GetLookupItems(string entityName)
        {
            return _service.CreateQuery(entityName, Context()).Select((entity) => new TypeEntity(entity, entityName));
        }

        public IEnumerable<TypeEntity> GetPickListItems(string entityName, string attributeName)
        {
            return _service.GetPickListItemsForAttribute(entityName, attributeName)
                .Select((pickListItem) => new TypeEntity(pickListItem, entityName, attributeName));
        }

        public IEnumerable<PrivacyPolicy> GetPrivacyPolicies()
        {
            return _service.CreateQuery("dfe_privacypolicy", Context())
                .Where((entity) =>
                    entity.GetAttributeValue<OptionSetValue>("dfe_policytype").Value == (int)PrivacyPolicyType.Web &&
                    entity.GetAttributeValue<bool>("dfe_active"))
                .OrderByDescending((policy) => policy.GetAttributeValue<DateTime>("createdon"))
                .Select((entity) => new PrivacyPolicy(entity, this))
                .Take(MaximumNumberOfPrivacyPolicies);
        }

        public Candidate GetCandidate(ExistingCandidateRequest request)
        {
            var context = Context();
            var entity = _service.CreateQuery("contact", context)
                .Where(e =>
                    // Will perform a case-insensitive comparison
                    e.GetAttributeValue<string>("emailaddress1") == request.Email)
                .OrderByDescending(e => e.GetAttributeValue<DateTime>("createdon"))
                .Take(MaximumNumberOfCandidatesToMatch)
                .ToList()
                .FirstOrDefault(request.Match);

            if (entity == null)
            {
                return null;
            }

            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatequalification_ContactId"), context);
            _service.LoadProperty(entity, new Relationship("dfe_contact_dfe_candidatepastteachingposition_ContactId"), context);

            return new Candidate(entity, this);
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
                relatedEntity.Attributes[key.Replace($"{relationshipName}.", "")] =
                    entity.GetAttributeValue<AliasedValue>(key).Value;

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
            _service.SaveChanges(context);
            model.Id = entity.Id;
        }

        public IEnumerable<TeachingEvent> GetTeachingEvents()
        {
            var query = new QueryExpression("msevtmgt_event");
            query.ColumnSet.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(TeachingEvent)));

            var link = query.AddLink("msevtmgt_building", "msevtmgt_building", "msevtmgt_buildingid", JoinOperator.LeftOuter);
            link.Columns.AddColumns(BaseModel.EntityFieldAttributeNames(typeof(TeachingEventBuilding)));
            link.EntityAlias = "msevtmgt_event_building";

            var entities = _service.RetrieveMultiple(query);

            return entities.Select((entity) => new TeachingEvent(entity, this)).ToList();
        }

        private OrganizationServiceContext Context()
        {
            return _service.Context();
        }
    }
}
