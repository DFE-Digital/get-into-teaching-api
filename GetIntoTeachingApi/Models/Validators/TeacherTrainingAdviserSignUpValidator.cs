﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeacherTrainingAdviserSignUpValidator : AbstractValidator<TeacherTrainingAdviserSignUp>
    {
        public TeacherTrainingAdviserSignUpValidator(IStore store)
        {
            RuleFor(request => request.FirstName).NotNull();
            RuleFor(request => request.LastName).NotNull();
            RuleFor(request => request.Email).NotNull();
            RuleFor(request => request.DateOfBirth).NotNull();
            RuleFor(request => request.AcceptedPolicyId).NotNull();
            RuleFor(request => request.CountryId).NotNull();
            RuleFor(request => request.PreferredEducationPhaseId).NotNull();

            var unitedKingdomCountryGuid = new Guid("72f5c2e6-74f9-e811-a97a-000d3a2760f2");
            RuleFor(request => request.AddressLine1).NotEmpty().Unless(request => request.CountryId != unitedKingdomCountryGuid)
                .WithMessage("Must be set candidate in the UK.");
            RuleFor(request => request.AddressCity).NotEmpty().Unless(request => request.CountryId != unitedKingdomCountryGuid)
                .WithMessage("Must be set candidate in the UK.");
            RuleFor(request => request.AddressState).NotEmpty().Unless(request => request.CountryId != unitedKingdomCountryGuid)
                .WithMessage("Must be set candidate in the UK.");
            RuleFor(request => request.AddressPostcode).NotEmpty().Unless(request => request.CountryId != unitedKingdomCountryGuid)
                .WithMessage("Must be set candidate in the UK.");

            RuleFor(request => request.Telephone).NotEmpty()
                .When(request => request.PhoneCallScheduledAt != null)
                .WithMessage("Must be set to schedule a callback.");

            RuleFor(request => request.PhoneCallScheduledAt).NotNull()
                .When(request => request.DegreeStatusId == (int)CandidateQualification.DegreeStatus.HasDegreeEquivilent)
                .WithMessage("Must be set for candidate with UK equivilent degree.");

            RuleFor(request => request.InitialTeacherTrainingYearId).NotNull()
                .Unless(request => request.Candidate.PastTeachingPositions.Count > 0)
                .WithMessage("Must be set unless candidate has past teaching positions.");

            RuleFor(request => request.DegreeStatusId).NotEmpty()
                .Unless(request => request.Candidate.PastTeachingPositions.Count > 0)
                .WithMessage("Must be set unless candidate has past teaching positions.");

            RuleFor(request => request.DegreeSubject).NotEmpty()
                .When(request =>
                    new List<int?>
                    {
                        (int)CandidateQualification.DegreeStatus.HasDegree,
                        (int)CandidateQualification.DegreeStatus.IsStudying,
                    }.Contains(request.DegreeStatusId))
                .WithMessage("Must be set when candidate has a degree or is studying for a degree.");

            RuleFor(request => request.UkDegreeGradeId).NotEmpty()
                .When(request => request.DegreeStatusId == (int)CandidateQualification.DegreeStatus.HasDegree)
                .WithMessage("Must be set when candidate has a degree.");

            RuleFor(request => request.PreferredTeachingSubjectId).NotEmpty()
                .When(request => request.Candidate.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Secondary)
                .WithMessage("Must be set when preferred education phase is secondary.");

            RuleFor(request => request.PreferredEducationPhaseId)
                .Must(phase => phase == (int)Candidate.PreferredEducationPhase.Secondary)
                .When(request => request.Candidate.PastTeachingPositions.Count > 0)
                .WithMessage("Must be secondary when past teaching positions are present.");

            RuleFor(request => request)
                .Must(request => HasOrIsPlanningOnRetakingEnglish(request) &&
                HasOrIsPlanningOnRetakingMaths(request) && HasOrIsPlanningOnRetakingScience(request))
                .When(request => request.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Primary)
                .WithMessage("Must have or be retaking all GCSEs when preferred education phase is primary.");

            RuleFor(request => request)
                .Must(request => HasOrIsPlanningOnRetakingEnglish(request) && HasOrIsPlanningOnRetakingMaths(request))
                .When(request => request.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Secondary)
                .WithMessage("Must have or be retaking Maths and English GCSEs when preferred education phase is secondary.");

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store));
        }

        private static bool HasOrIsPlanningOnRetakingEnglish(TeacherTrainingAdviserSignUp request)
        {
            return new[]
            {
                request.HasGcseEnglishId,
                request.PlanningToRetakeGcseEnglishId,
            }.Any(value => (int?)Candidate.GcseStatus.HasOrIsPlanningOnRetaking == value);
        }

        private static bool HasOrIsPlanningOnRetakingMaths(TeacherTrainingAdviserSignUp request)
        {
            return new[]
            {
                request.HasGcseMathsId,
                request.PlanningToRetakeGcseMathsId,
            }.Any(value => (int?)Candidate.GcseStatus.HasOrIsPlanningOnRetaking == value);
        }

        private static bool HasOrIsPlanningOnRetakingScience(TeacherTrainingAdviserSignUp request)
        {
            return new[]
            {
                request.HasGcseScienceId,
                request.PlanningToRetakeCgseScienceId,
            }.Any(value => (int?)Candidate.GcseStatus.HasOrIsPlanningOnRetaking == value);
        }
    }
}