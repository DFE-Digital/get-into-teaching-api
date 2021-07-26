using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Models.SchoolsExperience;

namespace GetIntoTeachingApi.Models.SchoolsExperience.Validators
{
    public class ClassroomExperienceNoteValidator : AbstractValidator<ClassroomExperienceNote>
    {
        private static readonly int _urnLength = 6;
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
            RuleFor(request => request.SchoolUrn).NotEmpty().Must(urn => urn.ToString().Length == _urnLength);
            RuleFor(request => request.SchoolName).NotEmpty();
            RuleFor(request => request.RecordedAt).NotNull();
        }
    }
}