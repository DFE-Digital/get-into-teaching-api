using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidateQualificationValidator : AbstractValidator<CandidateQualification>
    {
        private readonly IStore _store;

        public CandidateQualificationValidator(IStore store)
        {
            _store = store;

            RuleFor(qualification => qualification.UkDegreeGradeId)
                .Must(id => UkDegreeGradeIds().Contains(id.ToString()))
                .Unless(qualification => qualification.UkDegreeGradeId == null)
                .WithMessage("Must be a valid qualification uk degree grade.");
            RuleFor(qualification => qualification.DegreeStatusId)
                .Must(id => StatusIds().Contains(id.ToString()))
                .Unless(qualification => qualification.DegreeStatusId == null)
                .WithMessage("Must be a valid qualification degree status.");
            RuleFor(qualification => qualification.TypeId)
                .Must(id => TypeIds().Contains(id.ToString()))
                .Unless(qualification => qualification.TypeId == null)
                .WithMessage("Must be a valid qualification types.");

            RuleFor(qualification => qualification.Subject).MaximumLength(600);
        }

        private IEnumerable<string> UkDegreeGradeIds()
        {
            return _store.GetPickListItems("dfe_candidatequalification", "dfe_ukdegreegrade").Select(grade => grade.Id);
        }

        private IEnumerable<string> StatusIds()
        {
            return _store.GetPickListItems("dfe_candidatequalification", "dfe_degreestatus").Select(status => status.Id);
        }

        private IEnumerable<string> TypeIds()
        {
            return _store.GetPickListItems("dfe_candidatequalification", "dfe_type").Select(type => type.Id);
        }
    }
}