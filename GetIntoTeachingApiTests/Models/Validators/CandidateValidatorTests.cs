using System;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;
using Guid = System.Guid;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class CandidateValidatorTests
    {
        private readonly CandidateValidator _validator;
        private readonly Mock<ICrmService> _mockCrm;

        public CandidateValidatorTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _validator = new CandidateValidator(_mockCrm.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };
            var mockPreferredTeachingSubject = NewMock(Guid.NewGuid());
            var mockPreferredEducationPhase = NewMock(111);
            var mockLocation = NewMock(222);
            var mockInitialTeacherTrainingYear = NewMock(333);

            _mockCrm
                .Setup(mock => mock.GetPrivacyPolicies())
                .Returns(new[] { mockPrivacyPolicy });
            _mockCrm
                .Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist"))
                .Returns(new[] { mockPreferredTeachingSubject });
            _mockCrm
                .Setup(mock => mock.GetPickListItems("contact", "dfe_preferrededucationphase01"))
                .Returns(new[] { mockPreferredEducationPhase });
            _mockCrm
                .Setup(mock => mock.GetPickListItems("contact", "dfe_isinuk"))
                .Returns(new[] { mockLocation });
            _mockCrm
                .Setup(mock => mock.GetPickListItems("contact", "dfe_ittyear"))
                .Returns(new[] { mockInitialTeacherTrainingYear });

            var candidate = new Candidate()
            {
                FirstName = "first",
                LastName = "last",
                Email = "email@address.com",
                DateOfBirth = DateTime.Now.AddYears(-18),
                Telephone = "07584 734 576",
                PhoneCallScheduledStartAt = DateTime.Now.AddDays(2),
                AcceptedPrivacyPolicyId = mockPrivacyPolicy.Id,
                PreferredTeachingSubjectId = mockPreferredTeachingSubject.Id,
                PreferredEducationPhaseId = mockPreferredEducationPhase.Id,
                LocationId = mockLocation.Id,
                InitialTeacherTrainingYearId = mockInitialTeacherTrainingYear.Id,
            };

            var result = _validator.TestValidate(candidate);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_AddressIsInvalid_HasError()
        {
            var candidate = new Candidate {Address = new Address {Line1 = ""}};
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.Address.Line1);
        }

        [Fact]
        public void Validate_QualificationIsInvalid_HasError()
        {
            var candidate = new Candidate { Qualifications = new [] { new CandidateQualification { TypeId = 123 } } };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor("Qualifications[0].TypeId");
        }

        [Fact]
        public void Validate_PastTeachingPositionIsInvalid_HasError()
        {
            var candidate = new Candidate { PastTeachingPositions = 
                new[]
                {
                    new CandidatePastTeachingPosition() { SubjectTaughtId = Guid.NewGuid() }
                }
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor("PastTeachingPositions[0].SubjectTaughtId");
        }

        [Fact]
        public void Validate_EmailAddressIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.Email, "");
        }

        [Fact]
        public void Validate_EmailAddressIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.Email, "invalid-email@");
        }

        [Fact]
        public void Validate_EmailAddressTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.Email, $"{new string('a', 50)}@{new string('a', 50)}.com");
        }

        [Fact]
        public void Validate_EmailAddressPresent_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.Email, "valid@email.com");
        }

        [Fact]
        public void Validate_DateOfBirthIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.DateOfBirth, null as DateTime?);
        }

        [Fact]
        public void Validate_DateOfBirthInFuture_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.DateOfBirth, DateTime.Now.AddDays(1));
        }

        [Fact]
        public void Validate_FirstNameIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.FirstName, "");
        }

        [Fact]
        public void Validate_FirstNameTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.FirstName, new string('a', 257));
        }

        [Fact]
        public void Validate_LastNameIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.LastName, "");
        }

        [Fact]
        public void Validate_LastNameTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.LastName, new string('a', 257));
        }

        [Fact]
        public void Validate_TelephoneIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.Telephone, "");
        }

        [Fact]
        public void Validate_TelephoneTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.Telephone, new string('a', 51));
        }

        [Fact]
        public void Validate_PhoneCallScheduledStartAtIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.PhoneCallScheduledStartAt, null as DateTime?);
        }

        [Fact]
        public void Validate_PhoneCallScheduledStartAtInPast_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.PhoneCallScheduledStartAt, DateTime.Now.AddDays(-1));
        }

        [Fact]
        public void Validate_AcceptedPrivacyPolicyIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.AcceptedPrivacyPolicyId, Guid.NewGuid());
        }

        [Fact]
        public void Validate_AcceptedPrivacyPolicyIdIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.AcceptedPrivacyPolicyId, null as Guid?);
        }

        [Fact]
        public void Validate_PreferredTeachingSubjectIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.PreferredTeachingSubjectId, Guid.NewGuid());
        }

        [Fact]
        public void Validate_PreferredTeachingSubjectIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.PreferredTeachingSubjectId, null as Guid?);
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.PreferredEducationPhaseId, 123);
        }

        [Fact]
        public void Validate_PreferredEducationPhaseIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.PreferredEducationPhaseId, null as int?);
        }

        [Fact]
        public void Validate_LocationIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.LocationId, 123);
        }

        [Fact]
        public void Validate_LocationIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.LocationId, null as int?);
        }

        [Fact]
        public void Validate_InitialTeacherTrainingYearIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.InitialTeacherTrainingYearId, 123);
        }

        [Fact]
        public void Validate_InitialTeacherTrainingYearIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.InitialTeacherTrainingYearId, null as int?);
        }

        private TypeEntity NewMock(dynamic id)
        {
            return new TypeEntity { Id = id };
        }
    }
}
