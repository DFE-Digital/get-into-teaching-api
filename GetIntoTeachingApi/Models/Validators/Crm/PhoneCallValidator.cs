using FluentValidation;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Validators.Crm
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