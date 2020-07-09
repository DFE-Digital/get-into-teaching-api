using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeachingEventSearchRequestValidator : AbstractValidator<TeachingEventSearchRequest>
    {
        private readonly IStore _store;

        public TeachingEventSearchRequestValidator(IStore store)
        {
            _store = store;

            RuleFor(request => request.Postcode)
                .Must(store.IsValidPostcode)
                .Unless(request => request.Postcode == null && request.Radius == null)
                .WithMessage("Must be a valid postcode.");
            RuleFor(request => request.TypeId)
                .Must(id => TypeIds().Contains(id.ToString()))
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

        private IEnumerable<string> TypeIds()
        {
            return _store.GetPickListItems("msevtmgt_event", "dfe_event_type").Select(type => type.Id);
        }
    }
}