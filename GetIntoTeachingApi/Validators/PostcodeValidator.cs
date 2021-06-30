using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Validators
{
    public class PostcodeValidator<T> : PropertyValidator<T, string>
    {
        public PostcodeValidator()
            : base()
        {
        }

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            if (value != null && Location.PostcodeRegex.IsMatch(value))
            {
                return true;
            }

            return false;
        }

        public override string Name => "PostcodeValidator";

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be a valid postcode.";
    }
}
