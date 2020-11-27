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

            RuleFor(request => request.PreferredEducationPhaseId)
                .NotNull()
                .Unless(request => request.SubjectTaughtId != null);

            RuleFor(request => request.AddressLine1).NotEmpty().Unless(request => request.CountryId != TypeEntity.UnitedKingdomCountryId)
                .WithMessage("Must be set candidate in the UK.");
            RuleFor(request => request.AddressCity).NotEmpty().Unless(request => request.CountryId != TypeEntity.UnitedKingdomCountryId)
                .WithMessage("Must be set candidate in the UK.");
            RuleFor(request => request.AddressPostcode).NotEmpty().Unless(request => request.CountryId != TypeEntity.UnitedKingdomCountryId)
                .WithMessage("Must be set candidate in the UK.");

            RuleFor(request => request.Telephone).NotEmpty()
                .When(request => request.PhoneCallScheduledAt != null)
                .WithMessage("Must be set to schedule a callback.");

            RuleFor(request => request.Telephone).NotEmpty()
                .When(request => request.DegreeTypeId == (int)CandidateQualification.DegreeType.DegreeEquivalent)
                .WithMessage("Must be set for candidates with an equivalent degree.");

            RuleFor(request => request.PhoneCallScheduledAt).NotNull()
                .When(request => request.DegreeTypeId == (int)CandidateQualification.DegreeType.DegreeEquivalent && request.CountryId == TypeEntity.UnitedKingdomCountryId)
                .WithMessage("Must be set for candidate with UK equivalent degree.");
            RuleFor(request => request.PhoneCallScheduledAt).Null()
                .When(request => request.CountryId != TypeEntity.UnitedKingdomCountryId)
                .WithMessage("Cannot be set for overseas candidates.");

            RuleFor(request => request.InitialTeacherTrainingYearId).NotNull()
                .Unless(request => request.Candidate.IsReturningToTeaching())
                .WithMessage("Must be set unless candidate has past teaching positions.");

            RuleFor(request => request.DegreeStatusId).NotEmpty()
                .Unless(request => request.Candidate.IsReturningToTeaching())
                .WithMessage("Must be set unless candidate has past teaching positions.");

            RuleFor(request => request.DegreeStatusId)
                .Must(status => status != (int)CandidateQualification.DegreeStatus.NoDegree)
                .WithMessage("Not eligible for service if degree status is no degree.");

            RuleFor(request => request.DegreeTypeId).NotEmpty()
                .Unless(request => request.Candidate.IsReturningToTeaching())
                .WithMessage("Must be set unless candidate has past teaching positions.");

            RuleFor(request => request.DegreeTypeId)
                .Must(type =>
                    new List<int?>
                    {
                        (int)CandidateQualification.DegreeType.Degree,
                        (int)CandidateQualification.DegreeType.DegreeEquivalent,
                    }.Contains(type))
                .When(request => request.DegreeStatusId == (int)CandidateQualification.DegreeStatus.HasDegree)
                .WithMessage("Must be set degree or degree equivalent when the degree status is has a degree.");

            RuleFor(request => request.DegreeTypeId)
                .Must(type => type == (int)CandidateQualification.DegreeType.Degree)
                .When(request =>
                    new List<int?>
                    {
                        (int)CandidateQualification.DegreeStatus.FinalYear,
                        (int)CandidateQualification.DegreeStatus.SecondYear,
                        (int)CandidateQualification.DegreeStatus.FirstYear,
                        (int)CandidateQualification.DegreeStatus.Other,
                    }.Contains(request.DegreeStatusId))
                .WithMessage("Must be set to degree when status is studying for a degree.");

            RuleFor(request => request.DegreeTypeId)
                .Must(type => type == (int)CandidateQualification.DegreeType.Degree)
                .When(request => request.DegreeStatusId == (int)CandidateQualification.DegreeStatus.NoDegree)
                .WithMessage("Must be set to degree when the degree status is no degree.");

            RuleFor(request => request.DegreeSubject).NotEmpty()
                .When(request =>
                    new List<int?>
                    {
                        (int)CandidateQualification.DegreeStatus.HasDegree,
                        (int)CandidateQualification.DegreeStatus.FinalYear,
                        (int)CandidateQualification.DegreeStatus.SecondYear,
                        (int)CandidateQualification.DegreeStatus.FirstYear,
                        (int)CandidateQualification.DegreeStatus.Other,
                    }.Contains(request.DegreeStatusId))
                .Unless(request => request.DegreeTypeId == (int)CandidateQualification.DegreeType.DegreeEquivalent)
                .WithMessage("Must be set when candidate has a degree or is studying for a degree.");

            RuleFor(request => request.UkDegreeGradeId).NotEmpty()
                .When(request =>
                    new List<int?>
                    {
                        (int)CandidateQualification.DegreeStatus.HasDegree,
                        (int)CandidateQualification.DegreeStatus.FinalYear,
                        (int)CandidateQualification.DegreeStatus.SecondYear,
                        (int)CandidateQualification.DegreeStatus.FirstYear,
                        (int)CandidateQualification.DegreeStatus.Other,
                    }.Contains(request.DegreeStatusId))
                .Unless(request => request.DegreeTypeId == (int)CandidateQualification.DegreeType.DegreeEquivalent)
                .WithMessage("Must be set when candidate has a degree or is studying for a degree (predicted grade).");

            RuleFor(request => request.PreferredTeachingSubjectId).NotEmpty()
                .When(request => request.Candidate.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Secondary)
                .WithMessage("Must be set when preferred education phase is secondary.");

            RuleFor(request => request)
                .Must(request => HasOrIsPlanningOnRetakingEnglishAndMaths(request) && HasOrIsPlanningOnRetakingScience(request))
                .When(request => request.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Primary)
                .Unless(request => request.DegreeTypeId == (int)CandidateQualification.DegreeType.DegreeEquivalent)
                .WithMessage("Must have or be retaking all GCSEs when preferred education phase is primary.");

            RuleFor(request => request)
                .Must(request => HasOrIsPlanningOnRetakingEnglishAndMaths(request))
                .When(request => request.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Secondary)
                .Unless(request => request.Candidate.IsReturningToTeaching() || request.DegreeTypeId == (int)CandidateQualification.DegreeType.DegreeEquivalent)
                .WithMessage("Must have or be retaking Maths and English GCSEs when preferred education phase is secondary.");

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store));
        }

        private static bool HasOrIsPlanningOnRetakingEnglishAndMaths(TeacherTrainingAdviserSignUp request)
        {
            return new[]
            {
                request.HasGcseMathsAndEnglishId,
                request.PlanningToRetakeGcseMathsAndEnglishId,
            }.Any(value => (int?)Candidate.GcseStatus.HasOrIsPlanningOnRetaking == value);
        }

        private static bool HasOrIsPlanningOnRetakingScience(TeacherTrainingAdviserSignUp request)
        {
            return new[]
            {
                request.HasGcseScienceId,
                request.PlanningToRetakeGcseScienceId,
            }.Any(value => (int?)Candidate.GcseStatus.HasOrIsPlanningOnRetaking == value);
        }
    }
}