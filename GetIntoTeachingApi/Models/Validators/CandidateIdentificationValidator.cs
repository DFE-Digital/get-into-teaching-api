using FluentValidation;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidateIdentificationValidator : AbstractValidator<CandidateIdentification>
    {
        public CandidateIdentificationValidator()
        {
            RuleFor(request => request.FirstName).NotEmpty();
            RuleFor(request => request.LastName).NotEmpty();
            RuleFor(request => request.Email).NotEmpty().EmailAddress();
        }
    }
}

