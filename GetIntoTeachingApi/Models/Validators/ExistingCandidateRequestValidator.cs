using FluentValidation;
using FluentValidation.Validators;

namespace GetIntoTeachingApi.Models.Validators
{
    public class ExistingCandidateRequestValidator : AbstractValidator<ExistingCandidateRequest>
    {
        public ExistingCandidateRequestValidator()
        {
            RuleFor(request => request.Email).NotEmpty().EmailAddress(EmailValidationMode.AspNetCoreCompatible).MaximumLength(100);
        }
    }
}