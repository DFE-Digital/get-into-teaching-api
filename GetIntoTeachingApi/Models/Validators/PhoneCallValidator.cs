using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class PhoneCallValidator : AbstractValidator<PhoneCall>
    {
        private readonly IStore _store;

        public PhoneCallValidator(IStore store)
        {
            _store = store;

            RuleFor(phoneCall => phoneCall.ScheduledAt).GreaterThan(candidate => DateTime.Now);
            RuleFor(phoneCall => phoneCall.Telephone).NotEmpty().MaximumLength(50);

            RuleFor(candidate => candidate.ChannelId)
                .Must(id => ChannelIds().Contains(id.ToString()))
                .WithMessage("Must be a valid candidate channel.");
        }

        private IEnumerable<string> ChannelIds()
        {
            return _store.GetPickListItems("phonecall", "dfe_channelcreation").Select(channel => channel.Id);
        }
    }
}