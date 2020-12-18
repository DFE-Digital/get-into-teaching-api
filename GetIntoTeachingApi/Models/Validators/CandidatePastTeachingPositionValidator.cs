using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidatePastTeachingPositionValidator : AbstractValidator<CandidatePastTeachingPosition>
    {
        public CandidatePastTeachingPositionValidator(IStore store)
        {
            RuleFor(position => position.EducationPhaseId)
                .SetValidator(new PickListItemIdValidator("dfe_candidatepastteachingposition", "dfe_educationphase", store));
            RuleFor(position => position.SubjectTaughtId)
                .SetValidator(new LookupItemIdValidator("dfe_teachingsubjectlist", store));
        }
    }
}