using System;
using FluentValidation;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeachingEventValidator : AbstractValidator<TeachingEvent>
    {

        public TeachingEventValidator()
        {
            RuleFor(teachingEvent => teachingEvent.Name).NotEmpty();
            RuleFor(teachingEvent => teachingEvent.Summary).NotEmpty();
            RuleFor(teachingEvent => teachingEvent.Description).NotEmpty();
            RuleFor(teachingEvent => teachingEvent.ProviderContactEmail).EmailAddress();
            RuleFor(teachingEvent => teachingEvent.StartAt).GreaterThan(DateTime.UtcNow);
            RuleFor(teachingEvent => teachingEvent.EndAt).GreaterThanOrEqualTo(rule => rule.StartAt);
        }
    }
}
