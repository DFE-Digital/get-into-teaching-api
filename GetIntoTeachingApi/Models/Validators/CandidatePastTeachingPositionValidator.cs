using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidatePastTeachingPositionValidator : AbstractValidator<CandidatePastTeachingPosition>
    {
        private readonly IStore _store;

        public CandidatePastTeachingPositionValidator(IStore store)
        {
            _store = store;

            RuleFor(position => position.EducationPhaseId)
                .Must(id => EducationPhaseIds().Contains(id.ToString()))
                .WithMessage("Must be a valid past teaching position education phase.");
            RuleFor(candidate => candidate.SubjectTaughtId)
                .Must(id => TeachingSubjectIds().Contains(id.ToString()))
                .WithMessage("Must be a valid teaching subject.");
        }

        private IEnumerable<string> TeachingSubjectIds()
        {
            return _store.GetTypeEntitites("dfe_teachingsubjectlist").Select(subject => subject.Id);
        }

        private IEnumerable<string> EducationPhaseIds()
        {
            return _store.
                GetTypeEntitites("dfe_candidatepastteachingposition", "dfe_educationphase")
                .Select(type => type.Id);
        }
    }
}