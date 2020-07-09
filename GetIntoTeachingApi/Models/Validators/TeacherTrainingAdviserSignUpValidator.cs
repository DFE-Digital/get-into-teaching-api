using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeacherTrainingAdviserSignUpValidator : AbstractValidator<TeacherTrainingAdviserSignUp>
    {
        public TeacherTrainingAdviserSignUpValidator(IStore store)
        {
            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store));
        }
    }
}