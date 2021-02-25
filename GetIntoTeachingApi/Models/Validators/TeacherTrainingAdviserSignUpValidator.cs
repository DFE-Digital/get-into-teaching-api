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
            RuleFor(request => request.FirstName).NotEmpty();
            RuleFor(request => request.LastName).NotEmpty();
            RuleFor(request => request.Email).NotEmpty();
            RuleFor(request => request.DateOfBirth).NotNull();
            RuleFor(request => request.AcceptedPolicyId).NotNull();
            RuleFor(request => request.CountryId).NotNull();
            RuleFor(request => request.TypeId).NotNull();

            When(request => request.Candidate.IsReturningToTeaching(), () =>
            {
                RuleFor(request => request.SubjectTaughtId).NotNull()
                    .WithMessage("Must be set for candidates returning to teacher training.");
                RuleFor(request => request.PreferredTeachingSubjectId).NotNull()
                    .WithMessage("Must be set for candidates returning to teacher training.");
            });

            When(request => request.Candidate.IsInterestedInTeaching(), () =>
            {
                RuleFor(request => request.PreferredEducationPhaseId).NotNull()
                    .WithMessage("Must be set for candidates interested in teacher training.");
                RuleFor(request => request.InitialTeacherTrainingYearId).NotNull()
                    .WithMessage("Must be set for candidates interested in teacher training.");
                RuleFor(request => request.DegreeStatusId).NotNull()
                    .WithMessage("Must be set for candidates interested in teacher training.");
                RuleFor(request => request.DegreeTypeId).NotNull()
                    .WithMessage("Must be set for candidates interested in teacher training.");

                RuleFor(request => request.PreferredTeachingSubjectId).NotNull()
                    .When(request => request.Candidate.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Secondary)
                    .WithMessage("Must be set when preferred education phase is secondary.");

                RuleFor(request => request.DegreeStatusId)
                    .Must(status => status != (int)CandidateQualification.DegreeStatus.NoDegree)
                    .WithMessage("Can not be no degree (ineligible for service).");

                RuleFor(request => request.DegreeTypeId)
                    .Must(type =>
                        new List<int?>
                        {
                            (int)CandidateQualification.DegreeType.Degree,
                            (int)CandidateQualification.DegreeType.DegreeEquivalent,
                        }.Contains(type))
                    .When(request => request.DegreeStatusId == (int)CandidateQualification.DegreeStatus.HasDegree)
                    .WithMessage("Must be set to degree or degree equivalent when the degree status is has a degree.");

                RuleFor(request => request.DegreeTypeId)
                    .Must(type => type == (int)CandidateQualification.DegreeType.Degree)
                    .When(request => StudyingForADegreeStatus().Contains(request.DegreeStatusId))
                    .WithMessage("Must be set to degree when status is studying for a degree.");

                Unless(request => request.DegreeTypeId == (int)CandidateQualification.DegreeType.DegreeEquivalent, () =>
                {
                    RuleFor(request => request)
                        .Must(request => HasOrIsPlanningOnRetakingEnglishAndMaths(request))
                        .When(request => request.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Secondary)
                        .WithMessage("Must have or be retaking Maths and English GCSEs when preferred education phase is secondary.");

                    RuleFor(request => request)
                        .Must(request => HasOrIsPlanningOnRetakingEnglishAndMaths(request) && HasOrIsPlanningOnRetakingScience(request))
                        .When(request => request.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Primary)
                        .WithMessage("Must have or be retaking all GCSEs when preferred education phase is primary.");

                    RuleFor(request => request.DegreeSubject).NotEmpty()
                        .When(request => HaveOrStudyingForADegreeStatus().Contains(request.DegreeStatusId))
                        .WithMessage("Must be set when candidate has a degree or is studying for a degree.");

                    RuleFor(request => request.UkDegreeGradeId).NotNull()
                        .When(request => HaveOrStudyingForADegreeStatus().Contains(request.DegreeStatusId))
                        .WithMessage("Must be set when candidate has a degree or is studying for a degree (predicted grade).");
                });

                When(request => request.DegreeTypeId == (int)CandidateQualification.DegreeType.DegreeEquivalent, () =>
                {
                    RuleFor(request => request.Telephone).NotEmpty()
                        .WithMessage("Must be set for candidates with an equivalent degree.");
                    RuleFor(request => request.PhoneCallScheduledAt).NotNull()
                        .When(request => request.CountryId == LookupItem.UnitedKingdomCountryId)
                        .WithMessage("Must be set for candidate with UK equivalent degree.");
                });
            });

            When(request => request.CountryId == LookupItem.UnitedKingdomCountryId, () =>
            {
                RuleFor(request => request.AddressLine1).NotEmpty().WithMessage("Must be set candidate in the UK.");
                RuleFor(request => request.AddressCity).NotEmpty().WithMessage("Must be set candidate in the UK.");
                RuleFor(request => request.AddressPostcode).NotEmpty().WithMessage("Must be set candidate in the UK.");
            });

            RuleFor(request => request.Telephone).NotEmpty()
                .When(request => request.PhoneCallScheduledAt != null)
                .WithMessage("Must be set to schedule a callback.");

            RuleFor(request => request.PhoneCallScheduledAt).Null()
                .When(request => request.CountryId != LookupItem.UnitedKingdomCountryId ||
                    request.DegreeTypeId != (int)CandidateQualification.DegreeType.DegreeEquivalent)
                .WithMessage("Can only be set for UK candidates with an equivalent degree.");

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store));
        }

        private static List<int?> StudyingForADegreeStatus()
        {
            return new List<int?>
            {
                (int)CandidateQualification.DegreeStatus.FinalYear,
                (int)CandidateQualification.DegreeStatus.SecondYear,
                (int)CandidateQualification.DegreeStatus.FirstYear,
                (int)CandidateQualification.DegreeStatus.Other,
            };
        }

        private static List<int?> HaveOrStudyingForADegreeStatus()
        {
            var status = StudyingForADegreeStatus();
            status.Add((int)CandidateQualification.DegreeStatus.HasDegree);
            return status;
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