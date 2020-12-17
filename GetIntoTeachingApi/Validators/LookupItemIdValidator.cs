using System;
using System.Linq;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Validators
{
    public class LookupItemIdValidator : PropertyValidator
    {
        private readonly string _entityName;
        private readonly IStore _store;

        public LookupItemIdValidator(string entityName, IStore store)
            : base("{PropertyName} must be a valid {EntityName} item.")
        {
            _entityName = entityName;
            _store = store;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var id = (Guid?)context.PropertyValue;

            var exists = _store.GetLookupItems(_entityName).Any(i => i.Id == id);

            if (exists)
            {
                return true;
            }

            context.MessageFormatter.AppendArgument("EntityName", _entityName);

            return false;
        }
    }
}
