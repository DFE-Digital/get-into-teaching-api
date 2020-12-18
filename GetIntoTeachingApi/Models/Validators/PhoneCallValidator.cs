using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class PhoneCallValidator : AbstractValidator<PhoneCall>
    {
        private readonly IStore _store;

        public PhoneCallValidator(IStore store)
        {
            _store = store;

            RuleFor(phoneCall => phoneCall.ScheduledAt).GreaterThan(candidate => DateTime.UtcNow);

            RuleFor(candidate => candidate.ChannelId)
                .Must(id => ChannelIds().Contains(id))
                .WithMessage("Must be a valid candidate channel.");
        }

        private IEnumerable<int?> ChannelIds()
        {
            return _store.GetPickListItems("phonecall", "dfe_channelcreation").Select(channel => (int?)channel.Id);
        }
    }
}