using System;
using Microsoft.Xrm.Sdk;
using MoreLinq;
using Newtonsoft.Json.Linq;

namespace GetIntoTeachingApiContractTests.Builders
{
    public static class CrmEntityBuilder {
        
        public static Entity FromToken(JToken token)
        {
            var name = token["LogicalName"]!.ToString();
            var guid = token["Id"] != null ? Guid.Parse(token["Id"].ToString()) : new Guid();

            var entity = new Entity(name, guid);
            
            var entityState = token["EntityState"]?.ToObject<int>();
            if (entityState != null)
                entity.EntityState = (EntityState) entityState;

            entity.RowVersion = token["RowVersion"]?.ToString();
            
            token["Attributes"]?.ForEach(CreateAttributes(entity));
            token["FormattedValues"]?.ForEach(CreateFormattedValues(entity));
            token["RelatedEntities"]?.ForEach(CreateRelatedEntityValues(entity));

            return entity;
        }

        private static Action<JToken> CreateRelatedEntityValues(Entity entity)
        {
            return attr =>
            {
                var relationship = attr["Key"]!;
                var related = attr["Value"]!;
                
                entity.RelatedEntities.Add(CreateRelationshipFromToken(relationship), CreateEntityCollectionFromToken(related));
            };
        }

        private static Action<JToken> CreateFormattedValues(Entity entity)
        {
            return attr =>
            {
                var key = attr["Key"]!.ToString();
                var value = attr["Value"]!.ToString();

                entity.FormattedValues.Add(key, value);
            };
        }

        private static Action<JToken> CreateAttributes(Entity entity)
        {
            return attr =>
            {
                var key = attr["Key"]!.ToString();
                
                // certain data attributes need a current date
                if (key == "dfe_timeofconsent" ||
                    key == "dfe_waitingtobeassigneddate" ||
                    key == "dfe_GITISTTAServiceStartDate")
                {
                    entity.Attributes.Add(key, DateTime.MinValue);
                    return;
                }
                
                // certain data attributes need a date 2 hours in the future
                if (key == "scheduledstart")
                {
                    entity.Attributes.Add(key, DateTime.UtcNow.AddHours(2));
                    return;
                }

                switch (attr["Value"]?.Type)
                {
                    case JTokenType.Integer:
                        entity.Attributes.Add(key, attr["Value"].Value<int>());
                        return;
                    case JTokenType.Float:
                        entity.Attributes.Add(key, attr["Value"].Value<float>());
                        return;
                    case JTokenType.String:
                        entity.Attributes.Add(key, attr["Value"].Value<string>());
                        return;
                    case JTokenType.Date:
                        entity.Attributes.Add(key, attr["Value"].Value<DateTime>());
                        return;
                    case JTokenType.Boolean:
                        entity.Attributes.Add(key, attr["Value"].Value<bool>());
                        return;
                }

                var value = attr["Value"]!.Value<JObject>();
                var attrValue = value.Property("Value");

                if (attrValue == null)
                {
                    entity.Attributes.Add(key, CreateEntityReferenceFromToken(value));
                    return;
                }

                entity.Attributes.Add(key, new OptionSetValue((int) attrValue.Value));
            };
        }

        private static EntityCollection CreateEntityCollectionFromToken(JToken related)
        {
            var collection = new EntityCollection
            {
                EntityName = related["EntityName"]!.ToString()
            };

            related["Entities"]?.ForEach(e =>
            {
                var entity = FromToken(e);
                entity.EntityState = EntityState.Created;
                collection.Entities.Add(entity);
            });
            
            return collection;
        }

        private static EntityReference CreateEntityReferenceFromToken(JToken token)
        {
            var name = token["LogicalName"]!.ToString();
            var guid = token["Id"] != null ? Guid.Parse(token["Id"].ToString()) : new Guid();

            return new EntityReference(name, guid);
        }

        private static Relationship CreateRelationshipFromToken(JToken token)
        {
            var name = token["SchemaName"]!.ToString();
            
            return new Relationship(name);
        }
    }
}