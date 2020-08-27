using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace GetIntoTeachingApi.Models.Validators
{
    public class ExistingCandidateRequestValidator : AbstractValidator<ExistingCandidateRequest>
    {
        public ExistingCandidateRequestValidator()
        {
            RuleFor(request => request.FirstName).MaximumLength(256);
            RuleFor(request => request.LastName).MaximumLength(256);
            RuleFor(request => request.Email).NotEmpty().EmailAddress(EmailValidationMode.AspNetCoreCompatible).MaximumLength(100);
            RuleFor(request => request.DateOfBirth).LessThan(request => DateTime.UtcNow);
            RuleFor(request => request)
                .Must(SpecifyTwoAdditionalRequiredAttributes)
                .WithMessage("You must specify values for 2 additional attributes (from birthdate, firstname and lastname).");
        }

        private static bool SpecifyTwoAdditionalRequiredAttributes(ExistingCandidateRequest request)
        {
            var additionalRequiredAttributes = new object[] { request.DateOfBirth, request.FirstName, request.LastName };
            return additionalRequiredAttributes.Count(attribute => attribute != null) >= 2;
        }
    }
}