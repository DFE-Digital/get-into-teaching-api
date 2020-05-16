using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class BaseModel
    {
        public Guid? Id { get; set; }

        public BaseModel() { }

        public BaseModel(Entity entity)
        {
            Id = entity.Id;
            MapEntityToProperties(this, entity);
        }

        public Entity ToEntity(Entity entity)
        {
            MapPropertiesToEntity(this, entity);
            return entity;
        }

        private static void MapEntityToProperties(BaseModel model, Entity entity)
        {
            foreach (var property in GetProperties(model))
            {
                var attribute = EntityAttribute(property);
                if (attribute == null) continue;

                if (attribute.Flatten)
                    property.SetValue(model, Activator.CreateInstance(property.PropertyType, entity));
                else if (attribute.Type == typeof(EntityReference))
                    property.SetValue(model, entity.GetAttributeValue<EntityReference>(attribute.Name)?.Id);
                else if (attribute.Type == typeof(OptionSetValue))
                    property.SetValue(model, entity.GetAttributeValue<OptionSetValue>(attribute.Name)?.Value);
                else
                    property.SetValue(model, entity.GetAttributeValue<dynamic>(attribute.Name));
            }
        }

        private static void MapPropertiesToEntity(object model, Entity entity)
        {
            foreach (var property in GetProperties(model))
            {
                var attribute = EntityAttribute(property);
                var value = property.GetValue(model);

                if (attribute == null || value == null) continue;

                if (attribute.Flatten)
                    MapPropertiesToEntity(value, entity);    
                else if (attribute.Type == typeof(EntityReference))
                    entity[attribute.Name] = new EntityReference(attribute.Reference, (Guid) value);
                else if (attribute.Type == typeof(OptionSetValue))
                    entity[attribute.Name] = new OptionSetValue((int) value);
                else
                    entity[attribute.Name] = value;
            }
        }

        private static IEnumerable<PropertyInfo> GetProperties(object model)
        {
            return model.GetType().GetProperties();
        }

        private static EntityAttribute EntityAttribute(ICustomAttributeProvider property)
        {
            return (EntityAttribute) property.GetCustomAttributes(false)
                .FirstOrDefault(a => a.GetType() == typeof(EntityAttribute));
        }
    }
}
