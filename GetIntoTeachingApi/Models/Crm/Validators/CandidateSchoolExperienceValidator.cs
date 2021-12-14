using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Crm.Validators
{
    public class CandidateSchoolExperienceValidator : AbstractValidator<CandidateSchoolExperience>
    {
        public CandidateSchoolExperienceValidator(IStore store)
        {
            RuleFor(se => se.SchoolUrn).MaximumLength(8);
            RuleFor(se => se.DurationOfPlacementInDays).LessThanOrEqualTo(100);
            RuleFor(se => se.TeachingSubjectId)
                .SetValidator(new LookupItemIdValidator<CandidateSchoolExperience>("dfe_teachingsubjectlist", store));
            RuleFor(se => se.Notes).MaximumLength(2000);
            RuleFor(se => se.SchoolName).MaximumLength(100);
        }
    }
}
