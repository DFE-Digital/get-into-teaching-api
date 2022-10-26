using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Adapters;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Moq;

namespace GetIntoTeachingApiTests.Helpers
{
    public class ContractTestOrganizationServiceAdapter : MockOrganizationServiceAdapter
    {
        public readonly List<Entity> TrackedEntities = new List<Entity>();

        public override Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context)
        {
            var existingEntity = base.BlankExistingEntity(entityName, id, context);
            TrackedEntities.Add(existingEntity);
            return existingEntity;
        }

        public override Entity NewEntity(string entityName, Guid? id, OrganizationServiceContext context)
        {
            var newEntity = base.NewEntity(entityName, id, context);
            TrackedEntities.Add(newEntity);
            return newEntity;
        }

        public override void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            // Related entities are nested against an already-tracked entity,
            // so they don't need to be tracked explicitly.
            source.Attributes[relationship.SchemaName] = target;
            TrackedEntities.Remove(target);
        }
    }
}
