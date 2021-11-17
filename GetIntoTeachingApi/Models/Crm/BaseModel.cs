using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Models.Crm
{
    public abstract class BaseModel : INotifyPropertyChanged
    {
        private readonly string[] _propertyNamesExcludedFromChangeTracking = new string[] { nameof(ChangedPropertyNames), nameof(HasUpfrontId) };
        private bool _changeTrackingEnabled = true;

        [NotMapped]
        [JsonProperty]
        public bool HasUpfrontId { get; private set; }
        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        public HashSet<string> ChangedPropertyNames { get; set; } = new HashSet<string>();
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid? Id { get; set; }

        protected BaseModel()
        {
            InitChangedPropertyNames();
        }

        protected BaseModel(Entity entity, ICrmService crm, IValidatorFactory vaidatorFactory)
            : this()
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

            var entity = MappableEntity(LogicalName(GetType()), crm, context);
            MapFieldAttributesToEntity(entity);
            FinaliseEntity(entity, crm, context);
            return entity;
        }

        public void DisableChangeTracking()
        {
            _changeTrackingEnabled = false;
        }

        public void EnableChangeTracking()
        {
            _changeTrackingEnabled = true;
        }

        public void GenerateUpfrontId()
        {
            if (Id.HasValue)
            {
                throw new InvalidOperationException("Can only generate IDs on models that do not have a preexisting ID.");
            }

            Id = Guid.NewGuid();
            HasUpfrontId = true;
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

        protected virtual void FinaliseEntity(Entity source, ICrmService crm, OrganizationServiceContext context)
        {
            // Hook.
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var excluded = _propertyNamesExcludedFromChangeTracking.Any(p => p == propertyName);

            if (_changeTrackingEnabled && !excluded)
            {
                ChangedPropertyNames.Add(propertyName);
            }
        }

        protected void DeleteLink(Entity source, ICrmService crm, OrganizationServiceContext context, BaseModel modelToRemove, string modelToRemovePropertyName)
        {
            PropertyInfo property = GetType().GetProperty(modelToRemovePropertyName);
            var attribute = EntityRelationshipAttribute(property);
            var target = modelToRemove.ToEntity(crm, context);
            crm.DeleteLink(source, new Relationship(attribute.Name), target, context);
        }

        private static IList NewListOfType(Type type)
        {
            var listType = typeof(List<>).MakeGenericType(type);
            return (IList)Activator.CreateInstance(listType);
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

        private Entity MappableEntity(string entityName, ICrmService crm, OrganizationServiceContext context)
        {
            return Id.HasValue && !HasUpfrontId
                ? crm.BlankExistingEntity(entityName, Id.Value, context)
                : crm.NewEntity(entityName, Id, context);
        }

        [OnDeserializing]
        private void DisableChangeTracking(StreamingContext context)
        {
            DisableChangeTracking();
        }

        [OnDeserialized]
        private void EnableChangeTracking(StreamingContext context)
        {
            EnableChangeTracking();
        }

        private void InitChangedPropertyNames()
        {
            // Adds any properties that are defined with a value in the model.
            var nonNullPropertyNames = GetType().GetProperties()
                .Where(p => p.CanWrite && p.GetValue(this) != null)
                .Select(p => p.Name);

            foreach (var name in nonNullPropertyNames)
            {
                NotifyPropertyChanged(name);
            }
        }

        private void NullifyInvalidFieldAttributes(IValidatorFactory validatorFactory)
        {
            var validator = validatorFactory.GetValidator(GetType());

            if (validator == null)
            {
                return;
            }

            var context = new ValidationContext<BaseModel>(this);
            var result = validator.Validate(context);

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
