using FluentValidation;

namespace GetIntoTeachingApi.Models.Validators
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(address => address.Line1).NotEmpty().MaximumLength(1024);
            RuleFor(address => address.Line2).MaximumLength(1024);
            RuleFor(address => address.Line3).MaximumLength(1024);
            RuleFor(address => address.City).NotEmpty().MaximumLength(128);
            RuleFor(address => address.State).NotEmpty().MaximumLength(128);
            RuleFor(address => address.Postcode).NotEmpty().MaximumLength(40);
        }
    }
}