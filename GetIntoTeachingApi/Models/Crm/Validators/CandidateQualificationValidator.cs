using System.Collections.Immutable;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Crm.Validators
{
    public class CandidateQualificationValidator : AbstractValidator<CandidateQualification>
    {

        public ImmutableList<CandidateQualification.UkDegreeGrade> ValidGrades { get; } = ImmutableList.Create( CandidateQualification.UkDegreeGrade.FirstClass, CandidateQualification.UkDegreeGrade.UpperSecond, CandidateQualification.UkDegreeGrade.LowerSecond);

        public CandidateQualificationValidator(IStore store)
        {
            RuleFor(qualification => qualification.UkDegreeGradeId)
                .SetValidator(new PickListItemIdValidator<CandidateQualification>("dfe_candidatequalification", "dfe_ukdegreegrade", store))
                .Must(qualification => qualification.HasValue && ValidGrades.Contains((CandidateQualification.UkDegreeGrade)qualification.Value))
                .Unless(qualification => qualification.UkDegreeGradeId == null)
                .WithMessage($"The UK degree grade must be one of: {string.Join(", ", ValidGrades.Select(g => g.ToString()))}.");
            RuleFor(qualification => qualification.DegreeStatusId)
                .SetValidator(new PickListItemIdValidator<CandidateQualification>("dfe_candidatequalification", "dfe_degreestatus", store))
                .Unless(qualification => qualification.DegreeStatusId == null);
            RuleFor(qualification => qualification.TypeId)
                .SetValidator(new PickListItemIdValidator<CandidateQualification>("dfe_candidatequalification", "dfe_type", store))
                .Unless(qualification => qualification.TypeId == null);
            RuleFor(qualification => qualification.DegreeCountry)
                .Must(degreeCountry => degreeCountry.HasValue && Country.DegreeCountriesList.Contains(degreeCountry))
                .When(qualification => qualification.DegreeCountry.HasValue)
                .Unless(qualification => qualification.DegreeCountry == null)
                .WithMessage("The selected country is not in the list of valid degree countries.");

            RuleFor(qualification => qualification.DegreeSubject).MaximumLength(600);
        }
    }
}