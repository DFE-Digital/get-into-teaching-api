using System;
using System.Collections.Generic;
using FluentValidation;
using GetIntoTeachingApi.Services;
using System.Linq;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidatePastTeachingPositionValidator : AbstractValidator<CandidatePastTeachingPosition>
    {
        private readonly ICrmService _crm;

        public CandidatePastTeachingPositionValidator(ICrmService crm)
        {
            _crm = crm;

            RuleFor(position => position.EducationPhaseId)
                .Must(id => EducationPhaseIds().Contains(id))
                .WithMessage("Must be a valid past teaching position education phase.");
            RuleFor(candidate => candidate.SubjectTaughtId)
                .Must(id => TeachingSubjectIds().Contains(id))
                .WithMessage("Must be a valid teaching subject.");
        }

        public IEnumerable<Guid?> TeachingSubjectIds()
        {
            return _crm.GetLookupItems("dfe_teachingsubjectlist").Select(subject => (Guid?)subject.Id);
        }

        public IEnumerable<int?> EducationPhaseIds()
        {
            return _crm.
                GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase")
                .Select(type => (int?)type.Id);
        }
    }
}