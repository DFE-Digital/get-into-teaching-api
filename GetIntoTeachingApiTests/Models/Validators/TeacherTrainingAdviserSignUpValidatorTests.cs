﻿using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class TeacherTrainingAdviserSignUpValidatorTests
    {
        private readonly TeacherTrainingAdviserSignUpValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public TeacherTrainingAdviserSignUpValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new TeacherTrainingAdviserSignUpValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var request = new TeacherTrainingAdviserSignUp()
            {
                CandidateId = Guid.NewGuid(),
                AcceptedPolicyId = Guid.NewGuid(),
                QualificationId = Guid.NewGuid(),
                SubjectTaughtId = Guid.NewGuid(),
                PastTeachingPositionId = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                CountryId = LookupItem.UnitedKingdomCountryId,
                UkDegreeGradeId = 1,
                DegreeStatusId = 2,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
                InitialTeacherTrainingYearId = 3,
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary,
                HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                HasGcseScienceId = 7,
                PlanningToRetakeGcseScienceId = 8,
                PlanningToRetakeGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.UtcNow,
                TeacherId = "abc123",
                DegreeSubject = "Maths",
                Telephone = "1234567",
                AddressLine1 = "Line 1",
                AddressLine2 = "Line 2",
                AddressCity = "City",
                AddressPostcode = "KY11 9YU",
                PhoneCallScheduledAt = DateTime.UtcNow,
            };

            var result = _validator.TestValidate(request);

            // Ensure no validation errors on root object (we expect errors on the Candidate
            // properties as we can't mock them).
            var propertiesWithErrors = result.Errors.Select(e => e.PropertyName);
            propertiesWithErrors.All(p => p.StartsWith("Candidate.")).Should().BeTrue();
        }

        [Fact]
        public void Validate_CandidateIsInvalid_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                FirstName = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("Candidate.FirstName");
        }

        [Fact]
        public void Validate_AcceptedPrivacyPolicyIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.AcceptedPolicyId, null as Guid?);
        }

        [Fact]
        public void Validate_CountryIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.CountryId, null as Guid?);
        }

        [Fact]
        public void Validate_TypeIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.TypeId, null as int?);
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIdIsNull_NotReturningToTeaching_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = null,
                SubjectTaughtId = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("PreferredEducationPhaseId");
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIdIsNull_ReturningToTeaching_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = null,
                SubjectTaughtId = Guid.NewGuid(),
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("PreferredEducationPhaseId");
        }

        [Fact]
        public void Validate_DateOfBirthIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.DateOfBirth, null as DateTime?);
        }

        [Fact]
        public void Validate_FirstNameIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.FirstName, null as string);
        }

        [Fact]
        public void Validate_LastNameIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.LastName, null as string);
        }

        [Fact]
        public void Validate_EmailIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(request => request.Email, null as string);
        }

        [Fact]
        public void Validate_CountryIsUkAndAddressIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                CountryId = LookupItem.UnitedKingdomCountryId,
                AddressLine1 = null,
                AddressCity = null,
                AddressPostcode = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("AddressLine1").WithErrorMessage("Must be set candidate in the UK.");
            result.ShouldHaveValidationErrorFor("AddressCity").WithErrorMessage("Must be set candidate in the UK.");
            result.ShouldHaveValidationErrorFor("AddressPostcode").WithErrorMessage("Must be set candidate in the UK.");
        }

        [Fact]
        public void Validate_CountryIsNotUkAndAddressIsNull_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                CountryId = new Guid("85f5c2e6-74f9-e811-a97a-000d3a2760f2"),
                AddressLine1 = null,
                AddressCity = null,
                AddressPostcode = null,
            };

            var result = _validator.TestValidate(request);
            
            result.ShouldNotHaveValidationErrorFor("AddressLine1");
            result.ShouldNotHaveValidationErrorFor("AddressCity");
            result.ShouldNotHaveValidationErrorFor("AddressPostcode");
        }

        [Fact]
        public void Validate_CountryIsUkAndAddressIsNotNull_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                CountryId = LookupItem.UnitedKingdomCountryId,
                AddressLine1 = "Line 1",
                AddressCity = "City",
                AddressPostcode = "KY11 9YU",
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("AddressLine1");
            result.ShouldNotHaveValidationErrorFor("AddressCity");
            result.ShouldNotHaveValidationErrorFor("AddressPostcode");
        }

        [Fact]
        public void Validate_PhoneCallScheduledAtIsPresentAndTelephoneIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PhoneCallScheduledAt = DateTime.UtcNow,
                Telephone = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("Telephone").WithErrorMessage("Must be set to schedule a callback.");
        }

        [Fact]
        public void Validate_DegreeTypeIdIsEquivalentAndTelephoneIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent,
                Telephone = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("Telephone").WithErrorMessage("Must be set for candidates with an equivalent degree.");
        }

        [Fact]
        public void Validate_DegreeTypeIdIsEquivalentAndTelephoneIsNullAndIsReturningToTeaching_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent,
                Telephone = null,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("Telephone");
        }

        [Fact]
        public void Validate_PhoneCallScheduledAtIsPresentAndTelephoneIsNotNull_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PhoneCallScheduledAt = DateTime.UtcNow,
                Telephone = "123456",
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("Telephone");
        }

        [Fact]
        public void Validate_DegreeTypeIsEquivalentAndCountryIsUkAndPhoneCallScheduledAtIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent,
                CountryId = LookupItem.UnitedKingdomCountryId,
                PhoneCallScheduledAt = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("PhoneCallScheduledAt").WithErrorMessage("Must be set for candidate with UK equivalent degree.");
        }

        [Fact]
        public void Validate_DegreeTypeIsEquivalentAndCountryIsUkAndPhoneCallScheduledAtIsNullAndReturningTeacher_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent,
                CountryId = LookupItem.UnitedKingdomCountryId,
                PhoneCallScheduledAt = null,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("PhoneCallScheduledAt");
        }

        [Fact]
        public void Validate_CountryIsNotUkAndPhoneCallScheduledAtIsNotNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                CountryId = Guid.NewGuid(),
                PhoneCallScheduledAt = DateTime.UtcNow,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("PhoneCallScheduledAt").WithErrorMessage("Cannot be set for overseas candidates.");
        }

        [Fact]
        public void Validate_DegreeTypeIsEquivalentAndCountryIsUkAndPhoneCallScheduledAtIsNotNull_HasNorror()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent,
                CountryId = LookupItem.UnitedKingdomCountryId,
                PhoneCallScheduledAt = DateTime.UtcNow,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("PhoneCallScheduledAt");
        }

        [Fact]
        public void Validate_DegreeStatusIsNoDegree_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.NoDegree,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("DegreeStatusId").WithErrorMessage("Not eligible for service if degree status is no degree.");
        }

        [Fact]
        public void Validate_DegreeStatusIsNoDegreeAndReturningTeacher_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.NoDegree,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeStatusId");
        }

        [Fact]
        public void Validate_DegreeStatusIsHasDegreeAndDegreeTypeIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                DegreeTypeId = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("DegreeTypeId").WithErrorMessage("Must be set degree or degree equivalent when the degree status is has a degree.");
        }

        [Fact]
        public void Validate_DegreeStatusIsHasDegreeAndDegreeTypeIsNullAndReturningTeacher_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                DegreeTypeId = null,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeTypeId");
        }

        [Fact]
        public void Validate_DegreeStatusIsHasDegreeAndDegreeTypeIsDegree_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                DegreeTypeId = (int)CandidateQualification.DegreeType.Degree,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeTypeId");
        }

        [Fact]
        public void Validate_DegreeStatusIsHasDegreeAndDegreeTypeIsDegreeEquivalent_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeTypeId");
        }

        [Fact]
        public void Validate_DegreeStatusIsStudyingAndAndDegreeTypeIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.FinalYear,
                DegreeTypeId = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("DegreeTypeId").WithErrorMessage("Must be set to degree when status is studying for a degree.");
        }

        [Fact]
        public void Validate_DegreeStatusIsStudyingAndAndDegreeTypeIsDegree_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.FinalYear,
                DegreeTypeId = (int)CandidateQualification.DegreeType.Degree,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeTypeId");
        }

        [Fact]
        public void Validate_DegreeStatusIsNoDegreeAndAndDegreeTypeIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.NoDegree,
                DegreeTypeId = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("DegreeTypeId").WithErrorMessage("Must be set to degree when the degree status is no degree.");
        }

        [Fact]
        public void Validate_DegreeStatusIsNoDegreeAndAndDegreeTypeIsNullAndReturningTeacher_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.NoDegree,
                DegreeTypeId = null,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeTypeId");
        }

        [Fact]
        public void Validate_DegreeStatusIsNoDegreeAndAndDegreeTypeIsDegree_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.NoDegree,
                DegreeTypeId = (int)CandidateQualification.DegreeType.Degree,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeTypeId");
        }

        [Fact]
        public void Validate_NoPastTeachingPositionsAndInitialTeacherTrainingYearIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                SubjectTaughtId = null,
                InitialTeacherTrainingYearId = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("InitialTeacherTrainingYearId").WithErrorMessage("Must be set unless candidate has past teaching positions.");
        }

        [Fact]
        public void Validate_NoPastTeachingPositionsAndInitialTeacherTrainingYearIsNotNull_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                SubjectTaughtId = null,
                InitialTeacherTrainingYearId = 1,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("InitialTeacherTrainingYearId");
        }

        [Fact]
        public void Validate_NoPastTeachingPositionsAndDegreeStatusIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                SubjectTaughtId = null,
                DegreeStatusId = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("DegreeStatusId").WithErrorMessage("Must be set unless candidate has past teaching positions.");
        }

        [Fact]
        public void Validate_NoPastTeachingPositionsAndDegreeStatusIsNotNull_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                SubjectTaughtId = null,
                DegreeStatusId = 1,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeStatusId");
        }

        [Fact]
        public void Validate_NoPastTeachingPositionsAndDegreeTypeIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                SubjectTaughtId = null,
                DegreeTypeId = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("DegreeTypeId").WithErrorMessage("Must be set unless candidate has past teaching positions.");
        }

        [Fact]
        public void Validate_NoPastTeachingPositionsAndDegreeTypeIsNotNull_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                SubjectTaughtId = null,
                DegreeTypeId = 1,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeTypeId");
        }

        [Fact]
        public void Validate_DegreeStatusIsStudyingAndDegreeSubjectIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.FirstYear,
                DegreeSubject = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("DegreeSubject").WithErrorMessage("Must be set when candidate has a degree or is studying for a degree.");
        }

        [Fact]
        public void Validate_DegreeStatusIsStudyingAndDegreeSubjectIsNullAndReturningTeacher_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.FirstYear,
                DegreeSubject = null,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeSubject");
        }

        [Fact]
        public void Validate_DegreeStatusIsHasDegreeAndDegreeTypeIsEquivalentAndDegreeSubjectIsNull_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent,
                DegreeSubject = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeSubject");
        }

        [Fact]
        public void Validate_DegreeStatusIsStudyingAndDegreeSubjectIsNotNull_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.FirstYear,
                DegreeSubject = "Maths",
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeSubject");
        }

        [Fact]
        public void Validate_DegreeStatusIsHasDegreeAndDegreeSubjectIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                DegreeSubject = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("DegreeSubject").WithErrorMessage("Must be set when candidate has a degree or is studying for a degree.");
        }

        [Fact]
        public void Validate_DegreeStatusIsHasDegreeAndDegreeSubjectIsNotNull_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                DegreeSubject = "English",
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("DegreeSubject");
        }

        [Fact]
        public void Validate_DegreeStatusIsHasDegreeAndUkDegreeGradeIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                UkDegreeGradeId = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("UkDegreeGradeId").WithErrorMessage("Must be set when candidate has a degree or is studying for a degree (predicted grade).");
        }

        [Fact]
        public void Validate_DegreeStatusIsHasDegreeAndUkDegreeGradeIsNullAndReturningTeacher_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                UkDegreeGradeId = null,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("UkDegreeGradeId");
        }

        [Fact]
        public void Validate_DegreeStatusIsHasDegreeAndDegreeTypeIsEquivalentAndUkDegreeGradeIsNull_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent,
                UkDegreeGradeId = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("UkDegreeGradeId");
        }

        [Fact]
        public void Validate_DegreeStatusIsHasDegreeAndUkDegreeGradeIsNotNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                UkDegreeGradeId = 1,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("UkDegreeGradeId");
        }

        [Fact]
        public void Validate_DegreeStatusIsStudyingAndUkDegreeGradeIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                UkDegreeGradeId = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("UkDegreeGradeId").WithErrorMessage("Must be set when candidate has a degree or is studying for a degree (predicted grade).");
        }

        [Fact]
        public void Validate_DegreeStatusIsStudyingAndUkDegreeGradeIsNotNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                DegreeStatusId = (int)CandidateQualification.DegreeStatus.HasDegree,
                UkDegreeGradeId = 1,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("UkDegreeGradeId");
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIsSecondaryAndPreferredTeachingSubjectIsNull_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary,
                PreferredTeachingSubjectId = null,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor("PreferredTeachingSubjectId").WithErrorMessage("Must be set when preferred education phase is secondary.");
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIsSecondaryAndPreferredTeachingSubjectIsNotNull_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary,
                PreferredTeachingSubjectId = Guid.NewGuid(),
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("PreferredTeachingSubjectId");
        }

        [Fact]
        public void Validate_HasPastTeachingPositionsAndPreferredEducationPhaseIsSecondary_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                SubjectTaughtId = Guid.NewGuid(),
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor("PreferredEducationPhaseId");
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIsPrimaryAndDoesNotHaveNorPlanningToRetakeAllGcses_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Primary,
                HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                HasGcseScienceId = -1,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request).WithErrorMessage("Must have or be retaking all GCSEs when preferred education phase is primary.");
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIsPrimaryAndDoesNotHaveNorPlanningToRetakeAllGcsesAndReturningTeacher_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Primary,
                HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                HasGcseScienceId = -1,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIsPrimaryAndHasOrIsPlanningToRetakeAllGcses_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Primary,
                HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                PlanningToRetakeGcseScienceId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIsPrimaryAndDoesNotHaveNorPlanningToRetakeAllGcsesAndIsEquivalentDegree_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Primary,
                HasGcseMathsAndEnglishId = -1,
                DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIsSecondaryAndDoesNotHaveNorPlanningToRetakeMathsAndEnglishGcsesAndIsReturningToTeaching_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
                HasGcseMathsAndEnglishId = -1,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIsSecondaryAndDoesNotHaveNorPlanningToRetakeMathsAndEnglishGcsesAndIsEquivalentDegree_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary,
                HasGcseMathsAndEnglishId = -1,
                DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIsSecondaryAndDoesNotHaveNorPlanningToRetakeMathsAndEnglishGcses_HasError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary,
                HasGcseMathsAndEnglishId = -1,
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(request => request).WithErrorMessage("Must have or be retaking Maths and English GCSEs when preferred education phase is secondary.");
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIsSecondaryAndHasOrIsPlanningToRetakeMathsAndEnglishGcses_HasNoError()
        {
            var request = new TeacherTrainingAdviserSignUp
            {
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary,
                HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                PlanningToRetakeGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(request => request);
        }
    }
}
