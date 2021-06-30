using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Validators
{
    public class LookupItemIdValidator<T> : PropertyValidator<T, Guid?>
    {
        private readonly string _entityName;
        private readonly IStore _store;

        public LookupItemIdValidator(string entityName, IStore store)
            : base()
        {
            _entityName = entityName;
            _store = store;
        }

        public override bool IsValid(ValidationContext<T> context, Guid? value)
        {
            var exists = _store.GetLookupItems(_entityName).Any(i => i.Id == value);

            if (exists)
            {
                return true;
            }

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyName);
            context.MessageFormatter.AppendArgument("EntityName", _entityName);

            return false;
        }

        public override string Name => "LookupItemIdValidator";

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be a valid {EntityName} item.";
    }
}
