using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeachingEventRegistrationRequestValidator : AbstractValidator<TeachingEventRegistrationRequest>
    {
        public TeachingEventRegistrationRequestValidator(IStore store)
        {
            RuleFor(request => request.FirstName).NotEmpty().MaximumLength(256);
            RuleFor(request => request.LastName).NotEmpty().MaximumLength(256);
            RuleFor(request => request.Email).NotEmpty().EmailAddress(EmailValidationMode.AspNetCoreCompatible).MaximumLength(100);
            RuleFor(request => request.Telephone).MaximumLength(50);
            RuleFor(request => request.AddressPostcode).NotEmpty().MaximumLength(40);

            RuleFor(request => request.PrivacyPolicy).NotEmpty().SetValidator(new CandidatePrivacyPolicyValidator(store));
        }
    }
}