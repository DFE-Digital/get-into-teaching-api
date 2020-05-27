using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeachingSubjectValidator : AbstractValidator<TeachingSubject>
    {
        private readonly ICrmService _crm;

        public TeachingSubjectValidator(ICrmService crm)
        {
            _crm = crm;

            RuleFor(teachingSubject => teachingSubject.Id)
                .Must(id => TeachingSubjectIds().Contains(id))
                .Unless(teachingSubject => teachingSubject.Id == null)
                .WithMessage("Must be a valid teaching subject.");
        }

        private IEnumerable<Guid?> TeachingSubjectIds()
        {
            return _crm.GetLookupItems("dfe_teachingsubjectlist").Select(subject => (Guid?)subject.Id);
        }
    }
}