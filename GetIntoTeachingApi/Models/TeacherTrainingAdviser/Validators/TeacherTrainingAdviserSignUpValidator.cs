using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.Validators;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApi.Validators;
using Microsoft.AspNetCore.Mvc;

namespace GetIntoTeachingApi.Models.TeacherTrainingAdviser.Validators
{
    public class TeacherTrainingAdviserSignUpValidator : AbstractValidator<TeacherTrainingAdviserSignUp>, IValidatorInterceptor
    {
        public TeacherTrainingAdviserSignUpValidator(IStore store, IDateTimeProvider dateTime)
        {
            RuleFor(request => request.FirstName).NotNull();
            RuleFor(request => request.LastName).NotNull();
            RuleFor(request => request.Email).NotNull();
            RuleFor(request => request.DateOfBirth).NotNull();
            RuleFor(request => request.AcceptedPolicyId).NotNull();
            RuleFor(request => request.CountryId).NotNull();
            RuleFor(request => request.TypeId).NotNull();

            RuleFor(request => request.AddressTelephone).NotNull()
                .When(request => request.PhoneCallScheduledAt != null)
                .WithMessage("Must be set to schedule a callback.");
            RuleFor(request => request.PhoneCallScheduledAt).GreaterThan(candidate => dateTime.UtcNow)
                .When(request => request.PhoneCallScheduledAt != null)
                .WithMessage("Can only be scheduled for future dates.");

            RuleFor(request => request.PhoneCallScheduledAt).Null()
                .Unless(request => request.DegreeTypeId == (int)CandidateQualification.DegreeType.DegreeEquivalent)
                .WithMessage("Can only be set for candidates with an equivalent degree.");

            When(request => request.CountryId == Country.UnitedKingdomCountryId, () =>
            {
                RuleFor(request => request.AddressPostcode).NotNull().WithMessage("Must be set when the candidate is in the UK.");
            });

            When(request => request.Candidate.IsReturningToTeaching(), () =>
            {
                RuleFor(request => request.PreferredTeachingSubjectId).NotNull()
                    .When(request => request.Candidate.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Secondary)
                    .WithMessage("For candidates returning to teacher training, must be set when preferred education phase is secondary.");

                RuleFor(request => request.PreferredTeachingSubjectId).NotNull()
                    .When(request => request.Candidate.PreferredEducationPhaseId == null)
                    .WithMessage("For candidates returning to teacher training, must be set when preferred education phase defaults to secondary.");

                RuleFor(request => request.SubjectTaughtId).NotNull()
                    .When(request => request.StageTaughtId == null)
                    .WithMessage("For candidates returning to teacher training, must be set when stage taught defaults to secondary.");

                RuleFor(request => request.SubjectTaughtId).NotNull()
                    .When(request => request.StageTaughtId == (int)CandidatePastTeachingPosition.EducationPhase.Secondary)
                    .WithMessage("For candidates returning to teacher training, must be set when stage taught is secondary.");
            });

            When(request => request.Candidate.IsInterestedInTeaching(), () =>
            {
                RuleFor(request => request.PreferredEducationPhaseId).NotNull()
                    .When(request => request.DegreeStatusId == (int)DegreeStatus.HasDegree)
                    .WithMessage("Must be set for candidates interested in teacher training that have a degree.");

                RuleFor(request => request.DegreeTypeId).NotNull()
                    .WithMessage("Must be set for candidates interested in teacher training.");

                RuleFor(request => request.DegreeStatusId)
                    .Must(status => status != (int)DegreeStatus.NoDegree)
                    .WithMessage("Can not be no degree (ineligible for service).");

                RuleFor(request => request.PreferredTeachingSubjectId).NotNull()
                    .When(request => request.Candidate.PreferredEducationPhaseId == (int)Candidate.PreferredEducationPhase.Secondary)
                    .WithMessage("Must be set when preferred education phase is secondary.");

                RuleFor(request => request.InitialTeacherTrainingYearId).NotNull()
                    .When(request => request.DegreeStatusId == (int)DegreeStatus.HasDegree)
                    .WithMessage("Must be set for candidates interested in teacher training that have a degree.");

                RuleFor(request => request.DegreeTypeId)
                    .Must(type =>
                        new List<int?>
                        {
                            (int)CandidateQualification.DegreeType.Degree,
                            (int)CandidateQualification.DegreeType.DegreeEquivalent,
                        }.Contains(type))
                    .When(request => request.DegreeStatusId == (int)DegreeStatus.HasDegree)
                    .WithMessage("Must be set to degree or degree equivalent when the degree status is has a degree.");

                RuleFor(request => request.DegreeTypeId)
                    .Must(type => type == (int)CandidateQualification.DegreeType.Degree)
                    .When(request => StudyingForADegreeStatus().Contains(request.DegreeStatusId))
                    .WithMessage("Must be set to degree when status is studying for a degree.");

                Unless(request => request.DegreeTypeId == (int)CandidateQualification.DegreeType.DegreeEquivalent, () =>
                {
                    RuleFor(request => request.DegreeSubject).NotNull()
                        .WithMessage("Must be set when candidate has a degree or is studying for a degree.");
                });

                When(request => request.DegreeTypeId == (int)CandidateQualification.DegreeType.Degree &&
                    request.DegreeStatusId == (int)DegreeStatus.HasDegree, () =>
                    {
                        RuleFor(request => request)
                            .Must(request => HasOrIsPlanningOnRetakingEnglishAndMaths(request))
                            .When(request => request.PreferredEducationPhaseId != null)
                            .WithMessage("Must have or be retaking Maths and English GCSEs.");

                        RuleFor(request => request.UkDegreeGradeId).NotNull()
                            .WithMessage("Must be set when candidate has a degree.");
                    });
            });
            
            RuleFor(request => request.Situation)
                .SetValidator(new PickListItemIdValidator<TeacherTrainingAdviserSignUp>("contact", "dfe_situation", store))
                .Unless(request => request.Situation == null);
            RuleFor(request => request.Citizenship)
                .SetValidator(new PickListItemIdValidator<TeacherTrainingAdviserSignUp>("contact", "dfe_citizenship", store))
                .Unless(request => request.Citizenship == null);
            RuleFor(request => request.VisaStatus)
                .SetValidator(new PickListItemIdValidator<TeacherTrainingAdviserSignUp>("contact", "dfe_visastatus", store))
                .Unless(request => request.VisaStatus == null);
            RuleFor(request => request.Location)
                .SetValidator(new PickListItemIdValidator<TeacherTrainingAdviserSignUp>("contact", "dfe_location", store))
                .Unless(request => request.Location == null);
            RuleFor(request => request.DegreeCountry)
                .Must(degreeCountry => degreeCountry.HasValue && Country.DegreeCountriesList.Contains(degreeCountry))
                .When(qualification => qualification.DegreeCountry.HasValue)
                .Unless(qualification => qualification.DegreeCountry == null)
                .WithMessage("The selected country is not in the list of valid degree countries.");

            
            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store, dateTime));
        }

        public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
        {
            return commonContext;
        }

        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            return result.SurfaceErrorsOnMatchingProperties(validationContext);
        }

        private static List<int?> StudyingForADegreeStatus()
        {
            return new List<int?>
            {
                (int)DegreeStatus.FinalYear,
                (int)DegreeStatus.SecondYear,
                (int)DegreeStatus.FirstYear,
                (int)DegreeStatus.Other,
            };
        }

        private static bool HasOrIsPlanningOnRetakingEnglishAndMaths(TeacherTrainingAdviserSignUp request)
        {
            return new[]
            {
                request.HasGcseMathsAndEnglishId,
                request.PlanningToRetakeGcseMathsAndEnglishId,
            }.Any(value => (int?)Candidate.GcseStatus.HasOrIsPlanningOnRetaking == value);
        }
    }
}