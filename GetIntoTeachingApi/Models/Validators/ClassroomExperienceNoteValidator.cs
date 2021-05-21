using System.Linq;
using FluentValidation;

namespace GetIntoTeachingApi.Models.Validators
{
    public class ClassroomExperienceNoteValidator : AbstractValidator<ClassroomExperienceNote>
    {
        private static readonly string[] _validActions = new string[]
        {
            "REQUEST",
            "ACCEPTED",
            "ATTENDED",
            "DID NOT ATTEND",
            "CANCELLED BY SCHOOL",
            "CANCELLED BY CANDIDATE",
        };

        public ClassroomExperienceNoteValidator()
        {
            RuleFor(request => request.Action).NotEmpty().Must(a => _validActions.Contains(a));
            RuleFor(request => request.SchoolUrn).NotEmpty().MaximumLength(6);
            RuleFor(request => request.SchoolName).NotEmpty();
            RuleFor(request => request.RecordedAt).NotNull();
            RuleFor(request => request.Date).NotNull();
        }
    }
}