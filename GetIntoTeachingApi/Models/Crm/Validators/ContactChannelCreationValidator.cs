using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Crm.Validators
{
    public class ContactChannelCreationValidator : AbstractValidator<ContactChannelCreation>
    {
        public ContactChannelCreationValidator(IStore store)
        {
            RuleFor(contactChannelCreation => contactChannelCreation.CreationChannelSourceId)
                .SetValidator(new PickListItemIdValidator<ContactChannelCreation>("dfe_contactchannelcreation", "dfe_creationchannelsource", store));
            RuleFor(contactChannelCreation => contactChannelCreation.CreationChannelServiceId)
                .SetValidator(new PickListItemIdValidator<ContactChannelCreation>("dfe_contactchannelcreation", "dfe_creationchannelservice", store));
            RuleFor(contactChannelCreation => contactChannelCreation.CreationChannelActivityId)
                .SetValidator(new PickListItemIdValidator<ContactChannelCreation>("dfe_contactchannelcreation", "dfe_creationchannelactivities", store));
        }
    }
}