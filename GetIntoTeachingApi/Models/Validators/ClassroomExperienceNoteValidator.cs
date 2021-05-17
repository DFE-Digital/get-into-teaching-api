using FluentValidation;

namespace GetIntoTeachingApi.Models.Validators
{
    public class ClassroomExperienceNoteValidator : AbstractValidator<ClassroomExperienceNote>
    {
        private static readonly string ActionRegex =
            "\\A(REQUEST|ACCEPTED|ATTENDED|DID NOT ATTEND|CANCELLED BY (SCHOOL|CANDIDATE))\\z";

        public ClassroomExperienceNoteValidator()
        {
            RuleFor(request => request.Action).NotEmpty().Matches(ActionRegex);
            RuleFor(request => request.SchoolUrn).NotEmpty().MaximumLength(6);
            RuleFor(request => request.SchoolName).NotEmpty();
            RuleFor(request => request.RecordedAt).NotNull();
            RuleFor(request => request.Date).NotNull();
        }
    }
}