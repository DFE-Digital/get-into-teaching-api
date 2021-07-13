using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApi.Models.Validators.Crm
{
    public class TeachingEventValidator : AbstractValidator<TeachingEvent>
    {
        public TeachingEventValidator()
        {
            RuleFor(teachingEvent => teachingEvent.ReadableId).NotEmpty();
            RuleFor(teachingEvent => teachingEvent.Name).NotEmpty();
            RuleFor(teachingEvent => teachingEvent.ProviderContactEmail).EmailAddress(EmailValidationMode.AspNetCoreCompatible).MaximumLength(100);
            RuleFor(teachingEvent => teachingEvent.EndAt).GreaterThanOrEqualTo(rule => rule.StartAt);
        }
    }
}
