using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using GetIntoTeachingApi.Models.Crm.Validators;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GetIntoTeachingApi.Models.GetIntoTeaching.Validators
{
    public class TeachingEventAddAttendeeValidator : AbstractValidator<TeachingEventAddAttendee>, IValidatorInterceptor
    {
        public TeachingEventAddAttendeeValidator(IStore store, IDateTimeProvider dateTime)
        {
            RuleFor(request => request.FirstName).NotEmpty();
            RuleFor(request => request.LastName).NotEmpty();
            RuleFor(request => request.Email).NotEmpty();
            RuleFor(request => request.EventId).NotEmpty();
            RuleFor(request => request.AcceptedPolicyId).NotEmpty();
            RuleFor(request => request.ConsiderationJourneyStageId).NotNull().When(request => request.SubscribeToMailingList);
            RuleFor(request => request.DegreeStatusId).NotNull().When(request => request.SubscribeToMailingList);
            RuleFor(request => request.PreferredTeachingSubjectId).NotNull().When(request => request.SubscribeToMailingList);
            RuleFor(request => request.IsVerified).Must(isVerified => isVerified).Unless(request => request.IsWalkIn);

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