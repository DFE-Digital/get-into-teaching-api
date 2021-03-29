using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeachingEventValidator : AbstractValidator<TeachingEvent>
    {
        private readonly ICrmService _crm;

        public TeachingEventValidator(ICrmService crm, IDateTimeProvider dateTime)
        {
            _crm = crm;

            RuleFor(teachingEvent => teachingEvent.ReadableId)
                .NotEmpty()
                .Must(BeUnique)
                .When(teachingEvent => teachingEvent.Id == null)
                .WithMessage("Must be unique");
            RuleFor(teachingEvent => teachingEvent.Name).NotEmpty();
            RuleFor(teachingEvent => teachingEvent.ProviderContactEmail).EmailAddress().EmailAddress(EmailValidationMode.AspNetCoreCompatible).MaximumLength(100);
            RuleFor(teachingEvent => teachingEvent.StartAt).GreaterThan(dateTime.UtcNow);
            RuleFor(teachingEvent => teachingEvent.EndAt).GreaterThanOrEqualTo(rule => rule.StartAt);
        }

        private bool BeUnique(string readableId)
        {
            return _crm.GetTeachingEvent(readableId) == null;
        }
    }
}
