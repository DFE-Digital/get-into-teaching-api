﻿using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.TeacherTrainingAdviser;
using GetIntoTeachingApi.Models.TeacherTrainingAdviser.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.TeacherTrainingAdviser.Validators
{
    public class TeacherTrainingAdviserSignUpValidatorTests
    {
        private readonly TeacherTrainingAdviserSignUpValidator _validator;
        private readonly TeacherTrainingAdviserSignUp _request;

        public TeacherTrainingAdviserSignUpValidatorTests()
        {
            _request = new TeacherTrainingAdviserSignUp();
            _validator = new TeacherTrainingAdviserSignUpValidator(new Mock<IStore>().Object, new DateTimeProvider());
        }

        [Fact]
        public void Validate_WhenRequiredAttributesAreNull_HasErrors()
        {
            _request.FirstName = null;
            _request.LastName = null;
            _request.Email = null;
            _request.DateOfBirth = null;
            _request.AcceptedPolicyId = null;
            _request.CountryId = null;
            _request.TypeId = null;

            var result = _validator.TestValidate(_request);

            result.ShouldHaveValidationErrorFor(c => c.FirstName);
            result.ShouldHaveValidationErrorFor(c => c.LastName);
            result.ShouldHaveValidationErrorFor(c => c.Email);
            result.ShouldHaveValidationErrorFor(c => c.DateOfBirth);
            result.ShouldHaveValidationErrorFor(c => c.AcceptedPolicyId);
            result.ShouldHaveValidationErrorFor(c => c.CountryId);
            result.ShouldHaveValidationErrorFor(c => c.TypeId);
        }

        [Fact]
        public void Validate_WhenTelephoneIsNull_AndPhoneCallScheduledAtIsNotNull_HasError()
        {
            _request.AddressTelephone = null;
            _request.PhoneCallScheduledAt = DateTime.UtcNow.AddDays(1);

            var result = _validator.TestValidate(_request);

            result.ShouldHaveValidationErrorFor(request => request.AddressTelephone)
                .WithErrorMessage("Must be set to schedule a callback.");

            _request.AddressTelephone = "123456789";

            result = _validator.TestValidate(_request);

            result.ShouldNotHaveValidationErrorFor(request => request.AddressTelephone);

            _request.AddressTelephone = null;
            _request.PhoneCallScheduledAt = null;

            result = _validator.TestValidate(_request);

            result.ShouldNotHaveValidationErrorFor(request => request.AddressTelephone);
        }

        [Fact]
        public void Validate_WhenPhoneCallScheduledAtIsNotNull_AndDegreeTypeIsNotDegreeEquivalent_HasError()
        {
            _request.PhoneCallScheduledAt = DateTime.UtcNow.AddDays(1);
            _request.DegreeTypeId = (int)CandidateQualification.DegreeType.Degree;

            var result = _validator.TestValidate(_request);

            result.ShouldHaveValidationErrorFor(request => request.PhoneCallScheduledAt)
                .WithErrorMessage("Can only be set for candidates with an equivalent degree.");

            _request.PhoneCallScheduledAt = null;

            result = _validator.TestValidate(_request);

            result.ShouldNotHaveValidationErrorFor(request => request.AddressTelephone);
        }

        [Fact]
        public void Validate_WhenCountryIdIsUk_AndAddressPostcodeIsNull_HasError()
        {
            _request.CountryId = Country.UnitedKingdomCountryId;
            _request.AddressPostcode = null;

            var result = _validator.TestValidate(_request);

            result.ShouldHaveValidationErrorFor(request => request.AddressPostcode).WithErrorMessage("Must be set when the candidate is in the UK.");

            _request.CountryId = Guid.NewGuid();

            result = _validator.TestValidate(_request);

            result.ShouldNotHaveValidationErrorFor(request => request.AddressPostcode);

            _request.CountryId = Country.UnitedKingdomCountryId;
            _request.AddressPostcode = "TE5 1NG";

            result = _validator.TestValidate(_request);

            result.ShouldNotHaveValidationErrorFor(request => request.AddressPostcode);
        }

        public class ReturningToTeacherTraining
        {
            private readonly TeacherTrainingAdviserSignUpValidator _validator;
            private readonly TeacherTrainingAdviserSignUp _request;

            public ReturningToTeacherTraining()
            {
                _validator = new TeacherTrainingAdviserSignUpValidator(new Mock<IStore>().Object, new DateTimeProvider());
                _request = new TeacherTrainingAdviserSignUp() { TypeId = (int)Candidate.Type.ReturningToTeacherTraining };
            }

            [Fact]
            public void Validate_WhenValid_HasNoErrors()
            {
                _request.CandidateId = Guid.NewGuid();
                _request.PastTeachingPositionId = Guid.NewGuid();
                _request.AcceptedPolicyId = Guid.NewGuid();
                _request.CountryId = Guid.NewGuid();
                _request.SubjectTaughtId = Guid.NewGuid();
                _request.PreferredTeachingSubjectId = Guid.NewGuid();
                _request.FirstName = "John";
                _request.LastName = "Doe";
                _request.Email = "email@address.com";
                _request.DateOfBirth = DateTime.UtcNow;
                _request.TeacherId = "abc123";
                _request.AddressTelephone = "1234567";
                _request.AddressPostcode = "KY11 9YU";

                var result = _validator.TestValidate(_request);

                ShouldOnlyHaveValidationErrorsOnCandidateAttribute(result);
            }

            [Fact]
            public void Validate_WhenRequiredAttributesAreNull_HasErrors()
            {
                _request.SubjectTaughtId = null;
                _request.PreferredTeachingSubjectId = null;

                var result = _validator.TestValidate(_request);

                result.ShouldHaveValidationErrorFor(request => request.SubjectTaughtId)
                    .WithErrorMessage("For candidates returning to teacher training, must be set when stage taught defaults to secondary.");

                result.ShouldHaveValidationErrorFor(request => request.PreferredTeachingSubjectId)
                    .WithErrorMessage("For candidates returning to teacher training, must be set when preferred education phase is secondary.");
            }
            
            [Fact]
            public void Validate_WhenPreferredEducationPhaseNullRequiredAttributesAreNull_HasErrors()
            {
                _request.PreferredEducationPhaseId = null;
                _request.PreferredTeachingSubjectId = null;

                var result = _validator.TestValidate(_request);
                
                result.ShouldHaveValidationErrorFor(request => request.PreferredTeachingSubjectId)
                    .WithErrorMessage("For candidates returning to teacher training, must be set when preferred education phase is secondary.");
            }
            
            [Fact]
            public void Validate_WhenPreferredEducationPhasePrimaryRequiredAttributesAreNull_HasErrors()
            {
                _request.PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Primary;
                _request.PreferredTeachingSubjectId = null;

                var result = _validator.TestValidate(_request);
                
                result.ShouldNotHaveValidationErrorFor(request => request.PreferredTeachingSubjectId);
            }
            
            [Fact]
            public void Validate_WhenPreferredEducationPhaseSecondaryRequiredAttributesAreNull_HasErrors()
            {
                _request.PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary;
                _request.PreferredTeachingSubjectId = null;

                var result = _validator.TestValidate(_request);
                
                result.ShouldHaveValidationErrorFor(request => request.PreferredTeachingSubjectId)
                    .WithErrorMessage("For candidates returning to teacher training, must be set when preferred education phase is secondary.");
            }
            
            [Fact]
            public void Validate_WhenStageTaughtNullRequiredAttributesAreNull_HasErrors()
            {
                _request.StageTaughtId = null;
                _request.SubjectTaughtId = null;

                var result = _validator.TestValidate(_request);
                
                result.ShouldHaveValidationErrorFor(request => request.SubjectTaughtId)
                    .WithErrorMessage("For candidates returning to teacher training, must be set when stage taught defaults to secondary.");
            }
            
            [Fact]
            public void Validate_WhenStageTaughtPrimaryRequiredAttributesAreNull_HasErrors()
            {
                _request.StageTaughtId = (int)CandidatePastTeachingPosition.EducationPhase.Primary;
                _request.SubjectTaughtId = null;

                var result = _validator.TestValidate(_request);
                
                result.ShouldNotHaveValidationErrorFor(request => request.SubjectTaughtId);
            }
            
            [Fact]
            public void Validate_WhenStageTaughtSecondaryRequiredAttributesAreNull_HasErrors()
            {
                _request.StageTaughtId = (int)CandidatePastTeachingPosition.EducationPhase.Secondary;
                _request.SubjectTaughtId = null;

                var result = _validator.TestValidate(_request);
                
                result.ShouldHaveValidationErrorFor(request => request.SubjectTaughtId)
                    .WithErrorMessage("For candidates returning to teacher training, must be set when stage taught is secondary.");
            }
        }

        public class InterestedInTeacherTraining
        {
            private readonly TeacherTrainingAdviserSignUpValidator _validator;
            private readonly TeacherTrainingAdviserSignUp _request;

            public InterestedInTeacherTraining()
            {
                _validator = new TeacherTrainingAdviserSignUpValidator(new Mock<IStore>().Object, new DateTimeProvider());
                _request = new TeacherTrainingAdviserSignUp() { TypeId = (int)Candidate.Type.InterestedInTeacherTraining };
            }

            [Fact]
            public void Validate_WhenValid_HasNoErrors()
            {
                _request.CandidateId = Guid.NewGuid();
                _request.AcceptedPolicyId = Guid.NewGuid();
                _request.CountryId = Guid.NewGuid();
                _request.PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Primary;
                _request.InitialTeacherTrainingYearId = 0;
                _request.DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree;
                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.Degree;
                _request.PlanningToRetakeGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking;
                _request.HasGcseScienceId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking;
                _request.UkDegreeGradeId = 0;
                _request.DegreeSubject = "Maths";
                _request.FirstName = "John";
                _request.LastName = "Doe";
                _request.Email = "email@address.com";
                _request.DateOfBirth = DateTime.UtcNow;
                _request.AddressTelephone = "1234567";

                var result = _validator.TestValidate(_request);

                ShouldOnlyHaveValidationErrorsOnCandidateAttribute(result);
            }

            [Fact]
            public void Validate_WhenRequiredAttributesAreNull_HasErrors()
            {
                _request.InitialTeacherTrainingYearId = null;
                _request.DegreeStatusId = null;
                _request.DegreeTypeId = null;

                var result = _validator.TestValidate(_request);

                result.ShouldHaveValidationErrorFor(request => request.DegreeStatusId)
                    .WithErrorMessage("Must be set for candidates interested in teacher training.");

                result.ShouldHaveValidationErrorFor(request => request.DegreeTypeId)
                    .WithErrorMessage("Must be set for candidates interested in teacher training.");
            }

            [Fact]
            public void Validate_WhenDegreeStatusIdIsNoDegree_HasError()
            {
                _request.DegreeStatusId = (int)CandidateQualification.DegreeStatus.NoDegree;

                var result = _validator.TestValidate(_request);

                result.ShouldHaveValidationErrorFor(request => request.DegreeStatusId)
                    .WithErrorMessage("Can not be no degree (ineligible for service).");
            }

            [Fact]
            public void Validate_WhenPreferredEducationPhaseIdIsNull_AndHasADegree_HasError()
            {
                _request.PreferredEducationPhaseId = null;
                _request.DegreeStatusId = (int)CandidateQualification.DegreeStatus.FirstYear;

                var result = _validator.TestValidate(_request);

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.PreferredEducationPhaseId);

                _request.DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree;

                result = _validator.TestValidate(_request);

                result.ShouldHaveValidationErrorFor(request => request.PreferredEducationPhaseId)
                    .WithErrorMessage("Must be set for candidates interested in teacher training that have a degree.");
            }

            [Fact]
            public void Validate_WhenPreferredTeachingSubjectIdIsNull_AndPreferredEducationPhaseIdIsSecondary_HasError()
            {
                _request.PreferredTeachingSubjectId = null;
                _request.PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary;

                var result = _validator.TestValidate(_request);

                result.ShouldHaveValidationErrorFor(request => request.PreferredTeachingSubjectId)
                    .WithErrorMessage("Must be set when preferred education phase is secondary.");
            }

            [Fact]
            public void Validate_WhenDegreeTypeIdIsNotDegreeOrDegreeEquivalent_AndDegreeStatusIdIsHasDegree_HasError()
            {
                _request.DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree;
                _request.DegreeTypeId = 123;

                var result = _validator.TestValidate(_request);

                result.ShouldHaveValidationErrorFor(request => request.DegreeTypeId)
                    .WithErrorMessage("Must be set to degree or degree equivalent when the degree status is has a degree.");

                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.Degree;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.DegreeTypeId);

                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.DegreeTypeId);
            }

            [Fact]
            public void Validate_WhenDegreeTypeIdIsNotDegree_AndDegreeStatusIdIsStudyingForADegree_HasError()
            {
                _request.DegreeStatusId = (int)CandidateQualification.DegreeStatus.SecondYear;
                _request.DegreeTypeId = 123;

                var result = _validator.TestValidate(_request);

                result.ShouldHaveValidationErrorFor(request => request.DegreeTypeId)
                    .WithErrorMessage("Must be set to degree when status is studying for a degree.");

                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.Degree;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.DegreeTypeId);
            }

            [Fact]
            public void Validate_WhenNotHaveOrPlanningToRetakeGcseMathsAndEnglish_AndDegreeTypeIsHasDegreeNotStudying_AndPreferredEducationPhaseIsNotNull_HasError()
            {
                _request.HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.NotAnswered;
                _request.PlanningToRetakeGcseMathsAndEnglishId = (int)Candidate.GcseStatus.NotAnswered;
                _request.PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Primary;
                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.Degree;
                _request.DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree;

                var result = _validator.TestValidate(_request);

                result.ShouldHaveValidationErrorFor(request => request)
                    .WithErrorMessage("Must have or be retaking Maths and English GCSEs.");

                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request);

                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.Degree;
                _request.HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request);

                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.Degree;
                _request.HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.NotAnswered;
                _request.PlanningToRetakeGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request);

                _request.DegreeStatusId = (int)CandidateQualification.DegreeStatus.FinalYear;
                _request.HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.NotAnswered;
                _request.PlanningToRetakeGcseMathsAndEnglishId = (int)Candidate.GcseStatus.NotAnswered;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request);
            }

            [Fact]
            public void Validate_WhenDegreeSubjectIsNull_AndDegreeTypeIsNotDegreeEquivalent_HasError()
            {
                _request.DegreeSubject = null;
                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.Degree;

                var result = _validator.TestValidate(_request);

                result.ShouldHaveValidationErrorFor(request => request.DegreeSubject)
                    .WithErrorMessage("Must be set when candidate has a degree or is studying for a degree.");

                _request.DegreeSubject = "Maths";

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.DegreeSubject);

                _request.DegreeSubject = null;
                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.DegreeSubject);
            }

            [Fact]
            public void Validate_WhenUkDegreeGradeIsNull_AndDegreeTypeIsNotHasADegeree_HasError()
            {
                _request.UkDegreeGradeId = null;
                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.Degree;
                _request.DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree;

                var result = _validator.TestValidate(_request);

                result.ShouldHaveValidationErrorFor(request => request.UkDegreeGradeId)
                    .WithErrorMessage("Must be set when candidate has a degree.");

                _request.DegreeStatusId = (int)CandidateQualification.DegreeStatus.FirstYear;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.UkDegreeGradeId);

                _request.DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree;
                _request.UkDegreeGradeId = 0;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.UkDegreeGradeId);

                _request.UkDegreeGradeId = null;
                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.UkDegreeGradeId);
            }

            [Fact]
            public void Validate_WhenPhoneCallScheduledAtIsInPast_AndDegreeTypeIsDegreeEquivalent_AndCountryIdIsUk_HasError()
            {
                _request.PhoneCallScheduledAt = null;
                _request.CountryId = Country.UnitedKingdomCountryId;
                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent;

                var result = _validator.TestValidate(_request);

                _request.PhoneCallScheduledAt = DateTime.UtcNow.AddDays(-1);

                result = _validator.TestValidate(_request);

                result.ShouldHaveValidationErrorFor(request => request.PhoneCallScheduledAt)
                    .WithErrorMessage("Can only be scheduled for future dates.");

                _request.PhoneCallScheduledAt = DateTime.UtcNow.AddDays(1);

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.PhoneCallScheduledAt);

                _request.PhoneCallScheduledAt = null;
                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.Degree;

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.PhoneCallScheduledAt);

                _request.DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent;
                _request.CountryId = Guid.NewGuid();

                result = _validator.TestValidate(_request);

                result.ShouldNotHaveValidationErrorFor(request => request.PhoneCallScheduledAt);
            }
        }

        protected static void ShouldOnlyHaveValidationErrorsOnCandidateAttribute(
            TestValidationResult<TeacherTrainingAdviserSignUp> result)
        {
            // Ensure no validation errors on the request model itself.
            // We expect errors on the Candidate properties as we avoid mocking them.
            var propertiesWithErrors = result.Errors.Select(e => e.PropertyName);
            propertiesWithErrors.All(p => p.StartsWith("Candidate.")).Should().BeTrue();
        }
    }
}
