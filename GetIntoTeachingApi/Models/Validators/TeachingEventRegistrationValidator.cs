using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
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

            RuleFor(registration => registration.ChannelId)
                .Must(id => ChannelIds().Contains(id.ToString()))
                .Unless(registration => registration.Id != null)
                .WithMessage("Must be a valid teaching event registration channel.");
            RuleFor(regigstration => regigstration.ChannelId)
                .Must(id => id == null)
                .Unless(regigstration => regigstration.Id == null)
                .WithMessage("You cannot change the channel of an existing teaching event registration.");
        }

        private IEnumerable<string> ChannelIds()
        {
            return _store.GetTypeEntitites("msevtmgt_eventregistration", "dfe_channelcreation").Select(channel => channel.Id);
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
