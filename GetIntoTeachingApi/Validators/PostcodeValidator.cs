using FluentValidation.Validators;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Validators
{
    public class PostcodeValidator : PropertyValidator
    {
        public PostcodeValidator()
            : base("{PropertyName} must be a valid postcode.")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var postcode = (string)context.PropertyValue;

            if (postcode != null && Location.PostcodeRegex.IsMatch(postcode))
            {
                return true;
            }

            return false;
        }
    }
}
