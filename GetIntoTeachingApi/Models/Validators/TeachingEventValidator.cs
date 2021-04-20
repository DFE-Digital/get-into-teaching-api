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
                .When(teachingEvent => ReadableIdHasChanged(teachingEvent))
                .WithMessage("Must be unique");
            RuleFor(teachingEvent => teachingEvent.Name).NotEmpty();
            RuleFor(teachingEvent => teachingEvent.ProviderContactEmail).EmailAddress(EmailValidationMode.AspNetCoreCompatible).MaximumLength(100);
            RuleFor(teachingEvent => teachingEvent.EndAt).GreaterThanOrEqualTo(rule => rule.StartAt);
        }

        private bool ReadableIdHasChanged(TeachingEvent teachingEvent)
        {
            // Always changes on creation.
            if (teachingEvent.Id == null)
            {
                return true;
            }

            // Best effort check - Checking the CRM is much slower
            return !_store.TeachingEventExistsWithReadableId(teachingEvent.Id.Value, teachingEvent.ReadableId);
        }

        private bool BeUnique(string readableId)
        {
            return _crm.GetTeachingEvent(readableId) == null;
        }
    }
}
