using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Crm.Validators
{
    public class PhoneCallValidator : AbstractValidator<PhoneCall>
    {
        public PhoneCallValidator(IStore store)
        {
            RuleFor(phoneCall => phoneCall.ChannelId)
                .SetValidator(new PickListItemIdValidator<PhoneCall>("phonecall", "dfe_channelcreation", store))
                .Unless(phoneCall => phoneCall.ChannelId == null);
        }
    }
}