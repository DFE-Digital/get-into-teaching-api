using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace GetIntoTeachingApi.Adapters
{
    public interface IOrganizationServiceAdapter
    {
        string CheckStatus();
        IQueryable<Entity> CreateQuery(string entityName, OrganizationServiceContext context);
        IEnumerable<Entity> RetrieveMultiple(QueryBase query);
        void LoadProperty(Entity entity, Relationship relationship, OrganizationServiceContext context);
        IEnumerable<PickListItem> GetPickListItemsForAttribute(string entityName, string attributeName);

        /// <summary>
        /// Retrieves the defined values for a multi-select picklist field (multi-option set)
        /// from a Dataverse entity's metadata. Returns a list of PickListItem objects 
        /// containing the value ID and user-visible label.
        /// </summary>
        /// <param name="entityName">The logical name of the entity (e.g., "contact")</param>
        /// <param name="attributeName">The logical name of the multi-select field</param>
        /// <returns>List of PickListItem representing each option in the multi-select picklist</returns>
        IEnumerable<PickListItem> GetMultiSelectPickListItems(string entityName, string attributeName);

        IEnumerable<Entity> RelatedEntities(Entity entity, string attributeName);
        OrganizationServiceContext Context();
        Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context);
        Entity NewEntity(string entityName, Guid? id, OrganizationServiceContext context);
        void SaveChanges(OrganizationServiceContext context);
        void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
        void DeleteLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context);
    }
}
