﻿using FluentValidation;
using FluentValidation.Validators;

namespace GetIntoTeachingApi.Models.Crm.Validators
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
