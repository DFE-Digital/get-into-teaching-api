using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using GetIntoTeachingApi.Models.Crm.Validators;
using GetIntoTeachingApi.Models.SchoolsExperience;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GetIntoTeachingApi.Models.SchoolsExperience.Validators
{
    public class SchoolsExperienceSignUpValidator : AbstractValidator<SchoolsExperienceSignUp>, IValidatorInterceptor
    {
        public SchoolsExperienceSignUpValidator(IStore store, IDateTimeProvider dateTime)
        {
            RuleFor(request => request.PreferredTeachingSubjectId).NotNull();
            RuleFor(request => request.AcceptedPolicyId).NotNull();

            RuleFor(request => request.Email).NotEmpty();
            RuleFor(request => request.FirstName).NotEmpty();
            RuleFor(request => request.LastName).NotEmpty();
            RuleFor(request => request.DateOfBirth).NotNull();
            RuleFor(request => request.AddressLine1).NotNull();
            RuleFor(request => request.AddressCity).NotNull();
            RuleFor(request => request.AddressStateOrProvince).NotNull();
            RuleFor(request => request.AddressPostcode).NotNull();
            RuleFor(request => request.AddressTelephone).NotNull();
            RuleFor(request => request.HasDbsCertificate).NotNull();
            RuleFor(request => request.Telephone).NotNull();
            RuleFor(request => request.SecondaryTelephone).NotNull();

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store, dateTime));
        }

        public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
        {
            return commonContext;
        }

        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            return result.SurfaceErrorsOnMatchingProperties(validationContext);
        }
    }
}
