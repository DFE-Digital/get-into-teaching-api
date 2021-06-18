using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class GetIntoTeachingCallbackValidator : AbstractValidator<GetIntoTeachingCallback>
    {
        public GetIntoTeachingCallbackValidator(IStore store, IDateTimeProvider dateTime)
        {
            RuleFor(request => request.AcceptedPolicyId).NotNull();

            RuleFor(request => request.Email).NotEmpty();
            RuleFor(request => request.FirstName).NotEmpty();
            RuleFor(request => request.LastName).NotEmpty();
            RuleFor(request => request.AddressTelephone).NotNull();
            RuleFor(request => request.PhoneCallScheduledAt)
                .NotNull()
                .GreaterThan(candidate => dateTime.UtcNow)
                    .WithMessage("Can only be scheduled for future dates.");

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store, dateTime));
        }
    }
}
