using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeachingApi.Models
{
    public class BaseModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid? Id { get; set; }
        
        public BaseModel()
        {
        }

        public BaseModel(Entity entity, ICrmService crm)
        {
            Id = entity.Id;
            MapFieldAttributesFromEntity(entity);
            MapRelationshipAttributesFromEntity(entity, crm);
        }

        public virtual Entity ToEntity(ICrmService crm, OrganizationServiceContext context)
        {
            if (!ShouldMap(crm))
            {
                return null;
            }

            var entity = crm.MappableEntity(LogicalName(GetType()), Id, context);
            MapFieldAttributesToEntity(entity);
            MapRelationshipAttributesToEntity(entity, crm, context);
            return entity;
        }

        public static string[] EntityFieldAttributeNames(Type type)
        {
            var entityAttribute = (EntityAttribute) Attribute.GetCustomAttribute(type, typeof(EntityAttribute));
            var attributes = type.GetProperties().Select(EntityFieldAttribute).Where(a => a != null);
            var fieldNames = attributes.Select(a => a.Name);

            return fieldNames.Concat(new[] {$"{entityAttribute.LogicalName}id"}).ToArray();
        }

        public static EntityFieldAttribute EntityFieldAttribute(ICustomAttributeProvider property)
        {
            return (EntityFieldAttribute)property.GetCustomAttributes(false)
                .FirstOrDefault(a => a.GetType() == typeof(EntityFieldAttribute));
        }

        public static EntityRelationshipAttribute EntityRelationshipAttribute(ICustomAttributeProvider property)
        {
            return (EntityRelationshipAttribute)property.GetCustomAttributes(false)
                .FirstOrDefault(a => a.GetType() == typeof(EntityRelationshipAttribute));
        }

        public static string LogicalName(MemberInfo type)
        {
            var attribute = (EntityAttribute)Attribute.GetCustomAttribute(type, typeof(EntityAttribute));
            return attribute.LogicalName;
        }

        protected virtual bool ShouldMapRelationship(string propertyName, dynamic value, ICrmService crm)
        {
            // Hook.
            return true;
        }

        protected virtual bool ShouldMap(ICrmService crm)
        {
            // Hook.
            return true;
        }

        private void MapFieldAttributesFromEntity(Entity entity)
        {
            foreach (var property in GetProperties(this))
            {
                var attribute = EntityFieldAttribute(property);

                if (attribute == null)
                {
                    continue;
                }

                if (attribute.Type == typeof(EntityReference))
                {
                    property.SetValue(this, entity.GetAttributeValue<EntityReference>(attribute.Name)?.Id);
                }
                else if (attribute.Type == typeof(OptionSetValue))
                {
                    property.SetValue(this, entity.GetAttributeValue<OptionSetValue>(attribute.Name)?.Value);
                }
                else
                {
                    property.SetValue(this, entity.GetAttributeValue<dynamic>(attribute.Name));
                }
            }
        }

        private void MapFieldAttributesToEntity(Entity entity)
        {
            foreach (var property in GetProperties(this))
            {
                var attribute = EntityFieldAttribute(property);
                var value = property.GetValue(this);

                if (attribute == null || value == null)
                {
                    continue;
                }

                if (attribute.Type == typeof(EntityReference))
                {
                    entity[attribute.Name] = new EntityReference(attribute.Reference, (Guid) value);
                }
                else if (attribute.Type == typeof(OptionSetValue))
                {
                    entity[attribute.Name] = new OptionSetValue((int) value);
                }
                else
                {
                    entity[attribute.Name] = value;
                }
            }
        }

        private void MapRelationshipAttributesToEntity(Entity source, ICrmService crm, OrganizationServiceContext context)
        {
            foreach (var property in GetProperties(this))
            {
                var attribute = EntityRelationshipAttribute(property);
                var value = property.GetValue(this);
                var shouldMap = ShouldMapRelationship(property.Name, value, crm);

                if (attribute == null || value == null || !shouldMap)
                {
                    continue;
                }

                foreach (var relatedModel in EnumerableRelationshipModels(value))
                {
                    var target = relatedModel.ToEntity(crm, context);
                    if (target?.EntityState == EntityState.Created)
                    {
                        crm.AddLink(source, new Relationship(attribute.Name), target, context);
                    }
                }
            }
        }

        private void MapRelationshipAttributesFromEntity(Entity entity, ICrmService crm)
        {
            foreach (var property in GetProperties(this))
            {
                var attribute = EntityRelationshipAttribute(property);

                if (attribute == null)
                {
                    continue;
                }

                var relatedEntityAttribute = (EntityAttribute)Attribute.GetCustomAttribute(attribute.Type, typeof(EntityAttribute));
                var relatedEntities = crm.RelatedEntities(entity, attribute.Name, relatedEntityAttribute.LogicalName).ToList();

                if (!relatedEntities.Any())
                {
                    continue;
                }

                var relatedModels = relatedEntities.Select(e => Activator.CreateInstance(attribute.Type, e, crm));

                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    var list = NewListOfType(attribute.Type);
                    foreach (var model in relatedModels)
                    {
                        list.Add(model);
                    }

                    property.SetValue(this, list);
                }
                else
                    property.SetValue(this, relatedModels.FirstOrDefault());
            }
        }

        private static IList NewListOfType(Type type)
        {
            var listType = typeof(List<>).MakeGenericType(type);
            return (IList) Activator.CreateInstance(listType);
        }

        private static IEnumerable<BaseModel> EnumerableRelationshipModels(object relationship)
        {
            return relationship is BaseModel model ? new List<BaseModel> { model } : (IEnumerable<BaseModel>)relationship;
        }

        private static IEnumerable<PropertyInfo> GetProperties(object model)
        {
            return model.GetType().GetProperties();
        }
    }
}
