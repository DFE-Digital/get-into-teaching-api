using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Models;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using PickListItem = Microsoft.PowerPlatform.Dataverse.Client.Extensions.PickListItem;

namespace GetIntoTeachingApi.Adapters
{
    public class OrganizationServiceAdapter : IOrganizationServiceAdapter
    {
        private readonly ServiceClient _client;

        public OrganizationServiceAdapter(IOrganizationService client)
        {
            _client = (ServiceClient)client;
        }

        public string CheckStatus()
        {
            try
            {
                _client.GetEntityDisplayName("contact");
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return HealthCheckResponse.StatusOk;
        }

        public IQueryable<Entity> CreateQuery(string entityName, OrganizationServiceContext context)
        {
            return context.CreateQuery(entityName);
        }
        
        // TODO: write an improved retrieve method

        public IEnumerable<Entity> RetrieveMultiple(QueryBase query)
        {
            var collection = _client.RetrieveMultiple(query);
            return collection.Entities;
        }

        public void LoadProperty(Entity entity, Relationship relationship, OrganizationServiceContext context)
        {
            context.LoadProperty(entity, relationship);
        }

        public IEnumerable<Entity> RelatedEntities(Entity entity, string attributeName)
        {
            var result = new List<Entity>();

            var entities = entity.RelatedEntities
                .Where(pair => pair.Key.SchemaName == attributeName)
                .Select(pair => pair.Value.Entities)
                .FirstOrDefault();

            if (entities != null)
            {
                result.AddRange(entities);
            }

            return result;
        }

        public IEnumerable<Microsoft.PowerPlatform.Dataverse.Client.Extensions.PickListItem> GetPickListItemsForAttribute(
            string entityName,
            string attributeName)
        {
            var metaElement = _client.GetPickListElementFromMetadataEntity(entityName, attributeName);
            return metaElement.Items;
        }

        /// <summary>
        /// Retrieves the defined values for a multi-select picklist field (multi-option set)
        /// from a Dataverse entity's metadata. Returns a list of PickListItem objects 
        /// containing the value ID and user-visible label.
        /// </summary>
        /// <param name="entityName">The logical name of the entity (e.g., "contact")</param>
        /// <param name="attributeName">The logical name of the multi-select field</param>
        /// <returns>List of PickListItem representing each option in the multi-select picklist</returns>
        public IEnumerable<PickListItem> GetMultiSelectPickListItems(string entityName, string attributeName)
        {
            // Build a metadata request targeting a specific multi-select attribute.
            var request = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,               // Name of the table (entity).
                LogicalName = attributeName,                  // Name of the attribute (field).
                RetrieveAsIfPublished = true                  // Include unpublished changes (if any).
            };

            // Execute the request using the Dataverse service client.
            var response = (RetrieveAttributeResponse)_client.Execute(request);

            // Cast the response metadata to the expected multi-select type.
            if (response.AttributeMetadata is not MultiSelectPicklistAttributeMetadata attributeMetadata)
            {
                throw new InvalidCastException($"Attribute '{attributeName}' is not a multi-select picklist.");
            }

            // Safely retrieve the defined options from the attribute's OptionSet.
            OptionMetadataCollection options = attributeMetadata.OptionSet?.Options;

            // If no options are defined, return an empty list.
            if (options == null)
                return [];

            List<PickListItem> pickListItems = [];

            // Loop through each available option in the metadata.
            foreach (OptionMetadata option in options)
            {
                // Use label if available, otherwise fall back to a generic identifier.
                string label = option.Label?.UserLocalizedLabel?.Label ?? $"[Option {option.Value}]";
                // Use the option's value as the identifier, defaulting to -1 if not set.
                int identifier = option.Value ?? -1; 
                // Construct a PickListItem using metadata details.
                PickListItem pickListItem = new(label, identifier);

                pickListItems.Add(pickListItem); // Add the item to the output list.
            }

            return pickListItems;
        }

        public OrganizationServiceContext Context()
        {
            return new OrganizationServiceContext(_client);
        }

        public Entity BlankExistingEntity(string entityName, Guid id, OrganizationServiceContext context)
        {
            var existingEntity = new Entity(entityName, id);
            context.Attach(existingEntity);
            context.UpdateObject(existingEntity);
            return existingEntity;
        }

        public Entity NewEntity(string entityName, Guid? id, OrganizationServiceContext context)
        {
            var newEntity = id.HasValue ? new Entity(entityName, id.Value) : new Entity(entityName);
            context.AddObject(newEntity);
            return newEntity;
        }

        public void SaveChanges(OrganizationServiceContext context)
        {
            context.SaveChanges();
        }

        public void AddLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            context.AddLink(source, relationship, target);
        }

        public void DeleteLink(Entity source, Relationship relationship, Entity target, OrganizationServiceContext context)
        {
            context.DeleteLink(source, relationship, target);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return _client.Execute(request);
        }
    }
}
