using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace GetIntoTeachingApi.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        private readonly string[] _propertyNamesExcludedFromChangeTracking = new string[] { "ChangeTrackingEnabled", "ChangedPropertyNames" };

        [NotMapped]
        public HashSet<string> ChangedPropertyNames { get; set; } = new HashSet<string>();
        [JsonIgnore]
        [NotMapped]
        public bool ChangeTrackingEnabled { get; set; } = true;
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid? Id { get; set; }

        public BaseModel()
        {
        }

        public BaseModel(Entity entity, ICrmService crm, IValidatorFactory vaidatorFactory)
        {
            Id = entity.Id;

            MapFieldAttributesFromEntity(entity);
            MapRelationshipAttributesFromEntity(entity, crm, vaidatorFactory);
            NullifyInvalidFieldAttributes(vaidatorFactory);
        }

        #pragma warning disable 67
        public event PropertyChangedEventHandler PropertyChanged;
        #pragma warning restore 67

        public static string[] EntityFieldAttributeNames(Type type)
        {
            var entityAttribute = (EntityAttribute)Attribute.GetCustomAttribute(type, typeof(EntityAttribute));
            var attributes = type.GetProperties().Select(EntityFieldAttribute).Where(a => a != null);
            var fieldNames = attributes.Select(a => a.Name);

            return fieldNames.Concat(new[] { $"{entityAttribute.LogicalName}id" }).ToArray();
        }

        public static EntityFieldAttribute EntityFieldAttribute(ICustomAttributeProvider property)
        {
            return (EntityFieldAttribute)property.GetCustomAttributes(false)
                .FirstOrDefault(a => a.GetType() == typeof(EntityFieldAttribute) && !((EntityFieldAttribute)a).Ignored);
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

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (ChangeTrackingEnabled && !_propertyNamesExcludedFromChangeTracking.Any(p => p == propertyName))
            {
                ChangedPropertyNames.Add(propertyName);
            }
        }

        private static IList NewListOfType(Type type)
        {
            var listType = typeof(List<>).MakeGenericType(type);
            return (IList)Activator.CreateInstance(listType);
        }

        private static IEnumerable<BaseModel> EnumerableRelationshipModels(object relationship)
        {
            return relationship is BaseModel model ? new List<BaseModel> { model } : (IEnumerable<BaseModel>)relationship;
        }

        private static IEnumerable<PropertyInfo> GetProperties(object model)
        {
            return model.GetType().GetProperties();
        }

        private static string TrimAndNullifyIfEmpty(string input)
        {
            input = input?.Trim();
            return string.IsNullOrWhiteSpace(input) ? null : input;
        }

        private void NullifyInvalidFieldAttributes(IValidatorFactory validatorFactory)
        {
            var validator = validatorFactory.GetValidator(GetType());

            if (validator == null)
            {
                return;
            }

            var result = validator.Validate(this);

            foreach (var property in GetProperties(this))
            {
                var attribute = EntityFieldAttribute(property);

                if (attribute == null)
                {
                    continue;
                }

                if (result.Errors.Any(e => e.PropertyName == property.Name))
                {
                    property.SetValue(this, null);
                }
            }
        }

        [OnDeserializing]
        private void StopChangeTracking(StreamingContext context)
        {
            ChangeTrackingEnabled = false;
        }

        [OnDeserialized]
        private void StartChangeTracking(StreamingContext context)
        {
            ChangeTrackingEnabled = true;
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
                    var value = entity.GetAttributeValue<dynamic>(attribute.Name);

                    if (value is string @string)
                    {
                        value = TrimAndNullifyIfEmpty(@string);
                    }

                    property.SetValue(this, value);
                }
            }
        }

        private void MapFieldAttributesToEntity(Entity entity)
        {
            foreach (var property in GetProperties(this))
            {
                var attribute = EntityFieldAttribute(property);
                var value = property.GetValue(this);
                var valueChanged = ChangedPropertyNames.Contains(property.Name);

                if (attribute == null || !valueChanged)
                {
                    continue;
                }

                if (attribute.Type == typeof(EntityReference) && value != null)
                {
                    entity[attribute.Name] = new EntityReference(attribute.Reference, (Guid)value);
                }
                else if (attribute.Type == typeof(OptionSetValue) && value != null)
                {
                    entity[attribute.Name] = new OptionSetValue((int)value);
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
                var valueChanged = ChangedPropertyNames.Contains(property.Name);

                if (attribute == null || !valueChanged || value == null)
                {
                    continue;
                }

                foreach (var relatedModel in EnumerableRelationshipModels(value))
                {
                    var shouldMap = ShouldMapRelationship(property.Name, relatedModel, crm);
                    if (!shouldMap)
                    {
                        continue;
                    }

                    var target = relatedModel.ToEntity(crm, context);
                    if (target?.EntityState == EntityState.Created)
                    {
                        crm.AddLink(source, new Relationship(attribute.Name), target, context);
                    }
                }
            }
        }

        private void MapRelationshipAttributesFromEntity(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
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

                var relatedModels = relatedEntities.Select(e => Activator.CreateInstance(attribute.Type, e, crm, validatorFactory));

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
                {
                    property.SetValue(this, relatedModels.FirstOrDefault());
                }
            }
        }
    }
}
