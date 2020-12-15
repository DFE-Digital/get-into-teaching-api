﻿using System.Collections.Generic;
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
                .NotEmpty()
                .MaximumLength(40)
                .Matches(Location.OutwardOrFullPostcodeRegex)
                .Unless(request => request.Postcode == null && request.Radius == null);
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
            return _store.GetPickListItems("msevtmgt_event", "dfe_event_type").Select(type => (int?)type.Id);
        }
    }
}