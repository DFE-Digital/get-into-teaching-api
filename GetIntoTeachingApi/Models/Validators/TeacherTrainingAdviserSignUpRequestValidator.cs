using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeacherTrainingAdviserSignUpRequestValidator : AbstractValidator<TeacherTrainingAdviserSignUpRequest>
    {
        public TeacherTrainingAdviserSignUpRequestValidator(IStore store)
        {
            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store));
        }
    }
}