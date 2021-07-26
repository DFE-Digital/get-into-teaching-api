using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.GetIntoTeaching.Validators
{
    public class TeachingEventSearchRequestValidator : AbstractValidator<TeachingEventSearchRequest>
    {
        public TeachingEventSearchRequestValidator(IStore store)
        {
            RuleFor(request => request.Postcode)
                .NotEmpty()
                .MaximumLength(40)
                .Matches(Location.OutwardOrFullPostcodeRegex)
                .Unless(request => request.Postcode == null && request.Radius == null);
            RuleFor(request => request.TypeId)
                .SetValidator(new PickListItemIdValidator<TeachingEventSearchRequest>("msevtmgt_event", "dfe_event_type", store))
                .Unless(request => request.TypeId == null);
            RuleFor(request => request.Radius).GreaterThan(0);
            RuleFor(request => request)
                .Must(StartAfterEarlierThanStartBefore)
                .Unless(request => request.StartAfter == null || request.StartBefore == null)
                .WithMessage("Start after must be earlier than start before.");
        }

        private static bool StartAfterEarlierThanStartBefore(TeachingEventSearchRequest request)
        {
            return request.StartAfter < request.StartBefore;
        }
    }
}