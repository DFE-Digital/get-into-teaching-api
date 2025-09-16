using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Crm.Validators
{
    public class CandidateQualificationValidator : AbstractValidator<CandidateQualification>
    {
        public CandidateQualificationValidator(IStore store)
        {
            RuleFor(qualification => qualification.UkDegreeGradeId)
                .SetValidator(new PickListItemIdValidator<CandidateQualification>("dfe_candidatequalification", "dfe_ukdegreegrade", store))
                .Unless(qualification => qualification.UkDegreeGradeId == null);
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