using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidateQualificationValidator : AbstractValidator<CandidateQualification>
    {
        public CandidateQualificationValidator(IStore store)
        {
            RuleFor(qualification => qualification.UkDegreeGradeId)
                .SetValidator(new PickListItemIdValidator("dfe_candidatequalification", "dfe_ukdegreegrade", store))
                .Unless(qualification => qualification.UkDegreeGradeId == null);
            RuleFor(qualification => qualification.DegreeStatusId)
                .SetValidator(new PickListItemIdValidator("dfe_candidatequalification", "dfe_degreestatus", store))
                .Unless(qualification => qualification.DegreeStatusId == null);
            RuleFor(qualification => qualification.TypeId)
                .SetValidator(new PickListItemIdValidator("dfe_candidatequalification", "dfe_type", store))
                .Unless(qualification => qualification.TypeId == null);

            RuleFor(qualification => qualification.DegreeSubject).MaximumLength(600);
        }
    }
}