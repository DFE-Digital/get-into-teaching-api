using System;
using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeachingEventValidator : AbstractValidator<TeachingEvent>
    {
        private readonly ICrmService _crm;
        private readonly IStore _store;

        public TeachingEventValidator(ICrmService crm, IStore store)
        {
            _crm = crm;
            _store = store;

            RuleFor(teachingEvent => teachingEvent.ReadableId)
                .NotEmpty()
                .Must(BeUnique)
                .When(teachingEvent => teachingEvent.Id == null || ReadableIdHasChanged(teachingEvent))
                .WithMessage("Must be unique");
            RuleFor(teachingEvent => teachingEvent.Name).NotEmpty();
            RuleFor(teachingEvent => teachingEvent.ProviderContactEmail).EmailAddress(EmailValidationMode.AspNetCoreCompatible).MaximumLength(100);
            RuleFor(teachingEvent => teachingEvent.EndAt).GreaterThanOrEqualTo(rule => rule.StartAt);
        }

        private bool ReadableIdHasChanged(TeachingEvent teachingEvent)
        {
            if (teachingEvent.Id == null)
            {
                return false;
            }

            // Possible to have duplicate readableIds but chances are low and checking the CRM is slow
            var existingTeachingEvent = _store.GetTeachingEventAsync((Guid)teachingEvent.Id).GetAwaiter().GetResult();
            if (existingTeachingEvent != null)
            {
                return existingTeachingEvent.ReadableId != teachingEvent.ReadableId;
            }

            return false;
        }

        private bool BeUnique(string readableId)
        {
            return _crm.GetTeachingEvent(readableId) == null;
        }
    }
}
