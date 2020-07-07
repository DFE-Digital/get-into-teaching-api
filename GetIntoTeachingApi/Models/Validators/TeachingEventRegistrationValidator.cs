using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeachingEventRegistrationValidator : AbstractValidator<TeachingEventRegistration>
    {
        public TeachingEventRegistrationValidator(IStore store)
        {
            RuleFor(teachingEvent => teachingEvent.EventId)
                .Must(id => store.GetTeachingEventAsync(id).GetAwaiter().GetResult() != null)
                .WithMessage("Must be a valid teaching event.");
        }
    }
}
