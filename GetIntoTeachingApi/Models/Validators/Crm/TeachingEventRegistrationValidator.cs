using System;
using FluentValidation;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Validators.Crm
{
    public class TeachingEventRegistrationValidator : AbstractValidator<TeachingEventRegistration>
    {
        private readonly IStore _store;

        public TeachingEventRegistrationValidator(IStore store)
        {
            _store = store;

            RuleFor(registration => registration.EventId)
                .Must(id => BeAValidTeachingEvent(id))
                .WithMessage("Must be a valid teaching event.");

            RuleFor(registration => registration.EventId)
                .Must(id => BeAvailableForOnlineRegistrations(id))
                .WithMessage("Attendence cannot be registered for this event via the API (it has no WebFeedId).");
            RuleFor(regigstration => regigstration.ChannelId)
                .SetValidator(new PickListItemIdValidator<TeachingEventRegistration>("msevtmgt_eventregistration", "dfe_channelcreation", _store))
                .Unless(regigstration => regigstration.Id != null);
            RuleFor(regigstration => regigstration.ChannelId)
                .Must(id => id == null)
                .Unless(regigstration => regigstration.Id == null)
                .WithMessage("You cannot change the channel of an existing teaching event registration.");
        }

        private bool BeAvailableForOnlineRegistrations(Guid id)
        {
            var teachingEvent = _store.GetTeachingEventAsync(id).GetAwaiter().GetResult();

            return teachingEvent?.WebFeedId != null;
        }

        private bool BeAValidTeachingEvent(Guid id)
        {
            return _store.GetTeachingEventAsync(id).GetAwaiter().GetResult() != null;
        }
    }
}
