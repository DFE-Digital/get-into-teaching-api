using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeachingApi.Models
{
    public class BaseModel
    {
        public Guid? Id { get; set; }

        public BaseModel() { }

        public BaseModel(Entity entity, IOrganizationServiceAdapter service)
        {
            Id = entity.Id;
            MapFieldAttributesFromEntity(entity);
            MapRelationshipAttributesFromEntity(entity, service);
        }

        public Entity ToEntity(IOrganizationServiceAdapter service, OrganizationServiceContext context)
        {
            var entity = MappableEntity(service, context);
            MapFieldAttributesToEntity(entity);
            MapRelationshipAttributesToEntity(entity, service, context);
            return entity;
        }

        private void MapFieldAttributesFromEntity(Entity entity)
        {
            foreach (var property in GetProperties(this))
            {
                var attribute = EntityFieldAttribute(property);

                if (attribute == null) continue;

                if (attribute.Type == typeof(EntityReference))
                    property.SetValue(this, entity.GetAttributeValue<EntityReference>(attribute.Name)?.Id);
                else if (attribute.Type == typeof(OptionSetValue))
                    property.SetValue(this, entity.GetAttributeValue<OptionSetValue>(attribute.Name)?.Value);
                else
                    property.SetValue(this, entity.GetAttributeValue<dynamic>(attribute.Name));
            }
        }

        private void MapFieldAttributesToEntity(Entity entity)
        {
            foreach (var property in GetProperties(this))
            {
                var attribute = EntityFieldAttribute(property);
                var value = property.GetValue(this);

                if (attribute == null || value == null) continue;

                if (attribute.Type == typeof(EntityReference))
                    entity[attribute.Name] = new EntityReference(attribute.Reference, (Guid) value);
                else if (attribute.Type == typeof(OptionSetValue))
                    entity[attribute.Name] = new OptionSetValue((int) value);
                else
                    entity[attribute.Name] = value;
            }
        }

        private void MapRelationshipAttributesToEntity(Entity parent, IOrganizationServiceAdapter service, OrganizationServiceContext context)
        {
            foreach (var property in GetProperties(this))
            {
                var attribute = EntityRelationshipAttribute(property);
                var value = property.GetValue(this);

                if (attribute == null || value == null) continue;

                foreach (var model in EnumerableRelationshipModels(value))
                {
                    var child = model.ToEntity(service, context);
                    if (child.EntityState == EntityState.Created)
                        service.AddLink(parent, new Relationship(attribute.Name), child, context);
                }
            }
        }

        private void MapRelationshipAttributesFromEntity(Entity entity, IOrganizationServiceAdapter service)
        {
            foreach (var property in GetProperties(this))
            {
                var attribute = EntityRelationshipAttribute(property);

                if (attribute == null) continue;

                var relatedEntities = service.RelatedEntities(entity, attribute.Name);

                if (relatedEntities == null) continue;

                var relatedModels = relatedEntities.Select(e => Activator.CreateInstance(attribute.Type, e, service));

                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    var list = NewListOfType(attribute.Type);
                    foreach (var model in relatedModels) list.Add(model);
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

        private Entity MappableEntity(IOrganizationServiceAdapter service, OrganizationServiceContext context)
        {
            var entityName = EntityAttribute().LogicalName;
            return Id != null ? service.BlankExistingEntity(entityName, (Guid)Id, context) : service.NewEntity(entityName, context);
        }

        public IEnumerable<BaseModel> EnumerableRelationshipModels(object relationship)
        {
            return relationship is BaseModel model ? new List<BaseModel> { model } : (IEnumerable<BaseModel>)relationship;
        }

        private static IEnumerable<PropertyInfo> GetProperties(object model)
        {
            return model.GetType().GetProperties();
        }

        private static EntityFieldAttribute EntityFieldAttribute(ICustomAttributeProvider property)
        {
            return (EntityFieldAttribute)property.GetCustomAttributes(false)
                .FirstOrDefault(a => a.GetType() == typeof(EntityFieldAttribute));
        }

        private static EntityRelationshipAttribute EntityRelationshipAttribute(ICustomAttributeProvider property)
        {
            return (EntityRelationshipAttribute)property.GetCustomAttributes(false)
                .FirstOrDefault(a => a.GetType() == typeof(EntityRelationshipAttribute));
        }

        private EntityAttribute EntityAttribute()
        {
            return (EntityAttribute) Attribute.GetCustomAttribute(this.GetType(), typeof(EntityAttribute));
        }
    }
}
