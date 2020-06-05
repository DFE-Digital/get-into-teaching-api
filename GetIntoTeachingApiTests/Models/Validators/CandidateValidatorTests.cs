using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class CandidateValidatorTests
    {
        private readonly CandidateValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public CandidateValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new CandidateValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPreferredTeachingSubject = NewMock(Guid.NewGuid());
            var mockPreferredEducationPhase = NewMock(111);
            var mockLocation = NewMock(222);
            var mockInitialTeacherTrainingYear = NewMock(333);

            _mockStore
                .Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist"))
                .Returns(new[] { mockPreferredTeachingSubject });
            _mockStore
                .Setup(mock => mock.GetPickListItems("contact", "dfe_preferrededucationphase01"))
                .Returns(new[] { mockPreferredEducationPhase });
            _mockStore
                .Setup(mock => mock.GetPickListItems("contact", "dfe_isinuk"))
                .Returns(new[] { mockLocation });
            _mockStore
                .Setup(mock => mock.GetPickListItems("contact", "dfe_ittyear"))
                .Returns(new[] { mockInitialTeacherTrainingYear });

            var candidate = new Candidate()
            {
                FirstName = "first",
                LastName = "last",
                Email = "email@address.com",
                DateOfBirth = DateTime.Now.AddYears(-18),
                Telephone = "07584 734 576",
                AddressLine1 = "line1",
                AddressLine2 = "line2",
                AddressLine3 = "line3",
                AddressCity = "city",
                AddressState = "state",
                AddressPostcode = "postcode",
                PreferredTeachingSubjectId = Guid.Parse(mockPreferredTeachingSubject.Id),
                PreferredEducationPhaseId = int.Parse(mockPreferredEducationPhase.Id),
                LocationId = int.Parse(mockLocation.Id),
                InitialTeacherTrainingYearId = int.Parse(mockInitialTeacherTrainingYear.Id),
            };

            var result = _validator.TestValidate(candidate);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_QualificationIsInvalid_HasError()
        {
            var candidate = new Candidate
            {
                Qualifications = new List<CandidateQualification>
                {
                    new CandidateQualification {TypeId = 123}
                }
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor("Qualifications[0].TypeId");
        }

        [Fact]
        public void Validate_PastTeachingPositionIsInvalid_HasError()
        {
            var candidate = new Candidate
            {
                PastTeachingPositions =
                    new List<CandidatePastTeachingPosition>
                    {
                        new CandidatePastTeachingPosition {SubjectTaughtId = Guid.NewGuid()}
                    }
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor("PastTeachingPositions[0].SubjectTaughtId");
        }

        [Fact]
        public void Validate_PhoneCallIsInvalid_HasError()
        {
            var candidate = new Candidate
            {
                PhoneCall = new PhoneCall() { ScheduledAt = DateTime.Now.AddDays(-10) }
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.PhoneCall.ScheduledAt);
        }

        [Fact]
        public void Validate_PrivacyPolicyIsInvalid_HasError()
        {
            var candidate = new Candidate
            {
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() }
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.PrivacyPolicy.AcceptedPolicyId);
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
        public void Validate_AddressLine1IsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.AddressLine1, "");
        }

        [Fact]
        public void Validate_AddressLine1IsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.AddressLine1, new string('a', 1025));
        }

        [Fact]
        public void Validate_AddressLine2IsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.AddressLine2, new string('a', 1025));
        }

        [Fact]
        public void Validate_AddressLine3IsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.AddressLine3, new string('a', 1025));
        }

        [Fact]
        public void Validate_AddressCityIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.AddressCity, "");
        }

        [Fact]
        public void Validate_AddressCityIsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.AddressCity, new string('a', 129));
        }

        [Fact]
        public void Validate_AddressStateIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.AddressState, "");
        }

        [Fact]
        public void Validate_AddressStateIsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.AddressState, new string('a', 129));
        }

        [Fact]
        public void Validate_AddressPostcodeIsEmpty_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.AddressPostcode, "");
        }

        [Fact]
        public void Validate_AddressPostcodeIsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(address => address.AddressPostcode, new string('a', 41));
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

        private static TypeEntity NewMock(dynamic id)
        {
            return new TypeEntity { Id = id.ToString() };
        }
    }
}
