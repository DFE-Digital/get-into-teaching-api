using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Validators
{
    public class PickListItemIdsValidator<T> : PropertyValidator<T, int[]>
    {
        private readonly string _entityName;
        private readonly string _attributeName;
        private readonly IStore _store;

        public PickListItemIdsValidator(string entityName, string attributeName, IStore store)
        {
            _entityName = entityName;
            _attributeName = attributeName;
            _store = store;
        }

        public override bool IsValid(ValidationContext<T> context, int[] values)
        {
            foreach (int value in values)
            {
                if (!_store.GetPickListItems(_entityName, _attributeName).Any(i => i.Id == value))
                {
                    context.MessageFormatter.AppendArgument("PropertyName", context.PropertyName);
                    context.MessageFormatter.AppendArgument("EntityName", _entityName);
                    context.MessageFormatter.AppendArgument("AttributeName", _attributeName);

                    return false;
                }
            }

            return true;
        }

        public override string Name => "PickListItemIdsValidator";

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be valid {EntityName}/{AttributeName} items.";
    }
}
