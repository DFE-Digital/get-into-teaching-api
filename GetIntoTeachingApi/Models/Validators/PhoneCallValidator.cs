using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Validators
{
    public class PhoneCallValidator : AbstractValidator<PhoneCall>
    {
        public PhoneCallValidator(IStore store)
        {
            RuleFor(phoneCall => phoneCall.ChannelId)
                .SetValidator(new PickListItemIdValidator("phonecall", "dfe_channelcreation", store))
                .Unless(phoneCall => phoneCall.ChannelId == null);
        }
    }
}