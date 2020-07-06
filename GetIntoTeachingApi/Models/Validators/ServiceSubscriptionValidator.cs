using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class ServiceSubscriptionValidator : AbstractValidator<ServiceSubscription>
    {
        private readonly IStore _store;

        public ServiceSubscriptionValidator(IStore store)
        {
            _store = store;

            RuleFor(subscription => subscription.TypeId)
                .Must(id => TypeIds().Contains(id.ToString()))
                .WithMessage("Must be a valid service subscription type.");
        }

        private IEnumerable<string> TypeIds()
        {
            return _store.GetPickListItems("dfe_servicesubscription", "dfe_servicesubscriptiontype").Select(channel => channel.Id);
        }
    }
}
