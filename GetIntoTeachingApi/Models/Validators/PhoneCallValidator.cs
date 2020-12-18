using System;
using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Validators
{
    public class PhoneCallValidator : AbstractValidator<PhoneCall>
    {
        public PhoneCallValidator(IStore store)
        {
            RuleFor(phoneCall => phoneCall.ScheduledAt).GreaterThan(candidate => DateTime.UtcNow);
            RuleFor(phoneCall => phoneCall.ChannelId)
                .SetValidator(new PickListItemIdValidator("phonecall", "dfe_channelcreation", store));
        }
    }
}