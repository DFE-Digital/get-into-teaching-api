using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Validators
{
    public class CountryIdValidator<T> : PropertyValidator<T, Guid?>
    {
        private readonly IStore _store;

        public CountryIdValidator(IStore store)
            : base()
        {
            _store = store;
        }

        public override bool IsValid(ValidationContext<T> context, Guid? value)
        {
            var exists = _store.GetCountries().Any(i => i.Id == value);

            if (exists)
            {
                return true;
            }

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyPath);

            return false;
        }

        public override string Name => "CountryIdValidator";

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be a valid country id.";
    }
}
