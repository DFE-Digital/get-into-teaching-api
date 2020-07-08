using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeachingEventAddAttendeeRequestValidator : AbstractValidator<TeachingEventAddAttendeeRequest>
    {
        public TeachingEventAddAttendeeRequestValidator(IStore store)
        {
            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store));
        }
    }
}