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
                .Must(id => UkDegreeGradeIds().Contains(id))
                .Unless(qualification => qualification.UkDegreeGradeId == null)
                .WithMessage("Must be a valid qualification uk degree grade.");
            RuleFor(qualification => qualification.DegreeStatusId)
                .Must(id => StatusIds().Contains(id))
                .Unless(qualification => qualification.DegreeStatusId == null)
                .WithMessage("Must be a valid qualification degree status.");
            RuleFor(qualification => qualification.TypeId)
                .Must(id => TypeIds().Contains(id))
                .Unless(qualification => qualification.TypeId == null)
                .WithMessage("Must be a valid qualification types.");

            RuleFor(qualification => qualification.DegreeSubject).MaximumLength(600);
        }

        private IEnumerable<int?> UkDegreeGradeIds()
        {
            return _store.GetPickListItems("dfe_candidatequalification", "dfe_ukdegreegrade").Select(grade => (int?)grade.Id);
        }

        private IEnumerable<int?> StatusIds()
        {
            return _store.GetPickListItems("dfe_candidatequalification", "dfe_degreestatus").Select(status => (int?)status.Id);
        }

        private IEnumerable<int?> TypeIds()
        {
            return _store.GetPickListItems("dfe_candidatequalification", "dfe_type").Select(type => (int?)type.Id);
        }
    }
}