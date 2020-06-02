using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeachingEventSearchRequestValidator : AbstractValidator<TeachingEventSearchRequest>
    {
        private readonly ICrmService _crm;

        public TeachingEventSearchRequestValidator(ICrmService crm, IStore store)
        {
            _crm = crm;

            RuleFor(request => request.Postcode)
                .NotEmpty()
                .Must(store.IsValidPostcode)
                .WithMessage("Must be a valid postcode.");
            RuleFor(request => request.TypeId)
                .Must(id => TypeIds().Contains(id))
                .Unless(request => request.TypeId == null)
                .WithMessage("Must be a valid type.");
            RuleFor(request => request.Radius).GreaterThan(0);
            RuleFor(request => request)
                .Must(StartAfterEarlierThanStartBefore)
                .Unless(request => request.StartAfter == null)
                .WithMessage("Start after must be earlier than start before.");
        }

        private static bool StartAfterEarlierThanStartBefore(TeachingEventSearchRequest request)
        {
            return request.StartAfter < request.StartBefore;
        }

        private IEnumerable<int?> TypeIds()
        {
            return _crm.GetPickListItems("msevtmgt_event", "dfe_event_type").Select(type => (int?)type.Id);
        }
    }
}