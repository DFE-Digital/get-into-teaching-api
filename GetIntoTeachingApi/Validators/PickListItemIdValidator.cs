using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Validators
{
    public class PickListItemIdValidator<T> : PropertyValidator<T, int>
    {
        private readonly string _entityName;
        private readonly string _attributeName;
        private readonly IStore _store;

        public PickListItemIdValidator(string entityName, string attributeName, IStore store)
        {
            _entityName = entityName;
            _attributeName = attributeName;
            _store = store;
        }

        public override bool IsValid(ValidationContext<T> context, int value)
        {
            var exists = _store.GetPickListItems(_entityName, _attributeName).Any(i => i.Id == value);

            if (exists)
            {
                return true;
            }

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyName);
            context.MessageFormatter.AppendArgument("EntityName", _entityName);
            context.MessageFormatter.AppendArgument("AttributeName", _attributeName);

            return false;
        }

        public override string Name => "PickListItemIdValidator";

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be a valid {EntityName}/{AttributeName} item.";
    }
}
