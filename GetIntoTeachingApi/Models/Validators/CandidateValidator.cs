﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidateValidator : AbstractValidator<Candidate>
    {
        private readonly ICrmService _crm;

        public CandidateValidator(ICrmService crm)
        {
            _crm = crm;

            RuleFor(candidate => candidate.FirstName).NotEmpty().MaximumLength(256);
            RuleFor(candidate => candidate.LastName).NotEmpty().MaximumLength(256);
            RuleFor(candidate => candidate.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(candidate => candidate.DateOfBirth).NotNull().LessThan(candidate => DateTime.Now);
            RuleFor(candidate => candidate.Telephone).NotEmpty().MaximumLength(50);
            RuleFor(candidate => candidate.PhoneCallScheduledStartAt).NotNull().GreaterThan(candidate => DateTime.Now);

            RuleFor(candidate => candidate.Address)
                .SetValidator(new AddressValidator())
                .Unless(candidate => candidate.Address == null);
            RuleForEach(candidate => candidate.Qualifications).SetValidator(new CandidateQualificationValidator(crm));
            RuleForEach(candidate => candidate.PastTeachingPositions).SetValidator(new CandidatePastTeachingPositionValidator(crm));

            RuleFor(candidate => candidate.AcceptedPrivacyPolicyId)
                .Must(id => PrivacyPolicyIds().Contains(id))
                .WithMessage("Must be a valid privacy policy.");
            RuleFor(candidate => candidate.PreferredTeachingSubjectId)
                .Must(id => PreferredTeachingSubjectIds().Contains(id))
                .Unless(candidate => candidate.PreferredTeachingSubjectId == null)
                .WithMessage("Must be a valid teaching subject.");
            RuleFor(candidate => candidate.PreferredEducationPhaseId)
                .Must(id => PreferredEducationPhaseIds().Contains(id))
                .Unless(candidate => candidate.PreferredEducationPhaseId == null)
                .WithMessage("Must be a valid candidate education phase.");
            RuleFor(candidate => candidate.LocationId)
                .Must(id => LocationIds().Contains(id))
                .Unless(candidate => candidate.LocationId == null)
                .WithMessage("Must be a valid candidate location.");
            RuleFor(candidate => candidate.InitialTeacherTrainingYearId)
                .Must(id => InitialTeacherTrainingYearIds().Contains(id))
                .Unless(candidate => candidate.InitialTeacherTrainingYearId == null)
                .WithMessage("Must be a valid candidate initial teacher training year.");
        }

        public IEnumerable<Guid?> PreferredTeachingSubjectIds()
        {
            return _crm.GetLookupItems("dfe_teachingsubjectlist").Select(subject => (Guid?)subject.Id);
        }

        public IEnumerable<Guid?> PrivacyPolicyIds()
        {
            return _crm.GetPrivacyPolicies().Select(policy => (Guid?)policy.Id);
        }

        public IEnumerable<int?> PreferredEducationPhaseIds()
        {
            return _crm.GetPickListItems("contact", "dfe_preferrededucationphase01").Select(phase => (int?)phase.Id);
        }

        public IEnumerable<int?> LocationIds()
        {
            return _crm.GetPickListItems("contact", "dfe_isinuk").Select(location => (int?)location.Id);
        }

        public IEnumerable<int?> InitialTeacherTrainingYearIds()
        {
            return _crm.GetPickListItems("contact", "dfe_ittyear").Select(year => (int?)year.Id);
        }
    }
}