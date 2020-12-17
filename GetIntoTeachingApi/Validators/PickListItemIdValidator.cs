using System.Linq;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Validators
{
    public class PickListItemIdValidator : PropertyValidator
    {
        private readonly string _entityName;
        private readonly string _attributeName;
        private readonly IStore _store;

        public PickListItemIdValidator(string entityName, string attributeName, IStore store)
            : base("{PropertyName} must be a valid {EntityName}/{AttributeName} item.")
        {
            _entityName = entityName;
            _attributeName = attributeName;
            _store = store;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var id = (int?)context.PropertyValue;

            var exists = _store.GetPickListItems(_entityName, _attributeName).Any(i => i.Id == id);

            if (exists)
            {
                return true;
            }

            context.MessageFormatter.AppendArgument("EntityName", _entityName);
            context.MessageFormatter.AppendArgument("AttributeName", _attributeName);

            return false;
        }
    }
}
