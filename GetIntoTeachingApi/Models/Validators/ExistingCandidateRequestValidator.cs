using FluentValidation;
using System;
using System.Linq;

namespace GetIntoTeachingApi.Models.Validators
{
    public class ExistingCandidateRequestValidator : AbstractValidator<ExistingCandidateRequest>
    {
        public ExistingCandidateRequestValidator()
        {
            RuleFor(request => request.Email).NotEmpty().EmailAddress();
            RuleFor(request => request.DateOfBirth).LessThan(request => DateTime.Now);
            RuleFor(request => request)
                .Must(request => SpecifyTwoAdditionalRequiredAttributes(request))
                .WithMessage("You must specify values for 2 additional attributes (from birthdate, firstname and lastname).");
        }

        private Boolean SpecifyTwoAdditionalRequiredAttributes(ExistingCandidateRequest request)
        {
            var additionalRequiredAttributes = new Object[] { request.DateOfBirth, request.FirstName, request.LastName };
            return additionalRequiredAttributes.Where(attribute => attribute != null).Count() >= 2;
        }
    }
}

