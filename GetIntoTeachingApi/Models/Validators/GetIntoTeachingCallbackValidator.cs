using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using GetIntoTeachingApi.Models.Crm.Validators;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GetIntoTeachingApi.Models.Validators
{
    public class GetIntoTeachingCallbackValidator : AbstractValidator<GetIntoTeachingCallback>, IValidatorInterceptor
    {
        public GetIntoTeachingCallbackValidator(IStore store, IDateTimeProvider dateTime)
        {
            RuleFor(request => request.AcceptedPolicyId).NotNull();

            RuleFor(request => request.Email).NotEmpty();
            RuleFor(request => request.FirstName).NotEmpty();
            RuleFor(request => request.LastName).NotEmpty();
            RuleFor(request => request.AddressTelephone).NotNull();
            RuleFor(request => request.PhoneCallScheduledAt)
                .NotNull()
                .GreaterThan(_ => dateTime.UtcNow)
                    .WithMessage("Can only be scheduled for future dates.");

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
