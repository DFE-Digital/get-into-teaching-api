using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.Validators
{
    public class CandidateValidatorTests
    {
        private readonly CandidateValidator _validator;
        private readonly Mock<IStore> _mockStore;
        public CandidateValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new CandidateValidator(_mockStore.Object, new DateTimeProvider());
        }
        
        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockSubject = new TeachingSubject { Id = Guid.NewGuid() };
            var mockCountry = new Country { Id = Guid.NewGuid() };
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };
            int fakeId = new Faker().Random.Int();

            PickListItem fakePickListItem = new PickListItem() { Id = fakeId };

            _mockStore
                .Setup(mock => mock.GetTeachingSubjects())
                .Returns(new[] { mockSubject }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetCountries())
                .Returns(new[] { mockCountry }.AsQueryable());
            _mockStore
                .Setup(store => store.GetPickListItems(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new [] {fakePickListItem}.AsQueryable);
            _mockStore
                .Setup(mock => mock.GetPrivacyPolicies())
                .Returns(new[] { mockPrivacyPolicy }.AsQueryable());

            var candidate = new Candidate()
            {
                FirstName = "first",
                LastName = "last",
                Email = "email@candidate.com",
                DateOfBirth = DateTime.UtcNow.AddYears(-18),
                AddressTelephone = "07584 734 576",
                AddressLine1 = "line1",
                AddressLine2 = "line2",
                AddressLine3 = "line3",
                AddressCity = "city",
                AddressStateOrProvince = "county",
                AddressPostcode = "KY119YU",
                Telephone = "07584 734 576",
                SecondaryTelephone = "07584 734 576",
                MobileTelephone = "07584 734 576",
                HasDbsCertificate = true,
                HasGcseMathsId = fakeId,
                HasGcseEnglishId = fakeId,
                AdviserEligibilityId = fakeId,
                PlanningToRetakeGcseScienceId = fakeId,
                PlanningToRetakeGcseEnglishId = fakeId,
                TypeId = fakeId,
                AssignmentStatusId = fakeId,
                DoNotPostalMail = false,
                EligibilityRulesPassed = "true",
                ConsiderationJourneyStageId = fakeId,
                CountryId = mockCountry.Id,
                PreferredTeachingSubjectId = mockSubject.Id,
                PreferredEducationPhaseId = fakeId,
                InitialTeacherTrainingYearId = fakeId,
                ChannelId = fakeId,
                MailingListSubscriptionChannelId = fakeId,
                EventsSubscriptionChannelId = fakeId,
                TeacherTrainingAdviserSubscriptionChannelId = fakeId,
                ApplyPhaseId = fakeId,
                ApplyStatusId = fakeId,
                Situation = fakeId,
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id },
                Citizenship = fakeId,
                VisaStatus = fakeId
            };

            TestValidationResult<Candidate> result = _validator.TestValidate(candidate);
   
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_TechingEventRegistrationIsInvalid_HasError()
        {
            var candidate = new Candidate
            {
                TeachingEventRegistrations = new List<TeachingEventRegistration>
                {
                    new TeachingEventRegistration {EventId = Guid.NewGuid()}
                }
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor("TeachingEventRegistrations[0].EventId");
        }

        [Fact]
        public void Validate_QualificationIsInvalid_HasError()
        {
            var candidate = new Candidate
            {
                Qualifications = new List<CandidateQualification>
                {
                    new CandidateQualification {UkDegreeGradeId = 123}
                }
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor("Qualifications[0].UkDegreeGradeId");
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
                PhoneCall = new PhoneCall() { ChannelId = -1 }
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.PhoneCall.ChannelId);
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
        public void Validate_RequiredFieldsWhenNull_HasError()
        {
            var candidate = new Candidate()
            {
                Email = null,

            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public void Validate_EmailAddressInvalid_HasError()
        {
            var invalidEmail = "invalid-email@";
            var candidate = new Candidate() { Email = invalidEmail, SecondaryEmail = invalidEmail };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.Email);
            result.ShouldHaveValidationErrorFor(c => c.SecondaryEmail);
        }

        [Fact]
        public void Validate_EmailAddressTooLong_HasError()
        {
            var tooLongEmail = $"{new string('a', 50)}@{new string('a', 50)}.com";
            var candidate = new Candidate() { Email = tooLongEmail, SecondaryEmail = tooLongEmail };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.Email);
            result.ShouldHaveValidationErrorFor(c => c.SecondaryEmail);
        }

        [Fact]
        public void Validate_EmailAddressIsValid_HasNoError()
        {
            var validEmail = "valid@email.com";
            var candidate = new Candidate() { Email = validEmail, SecondaryEmail = validEmail };
            var result = _validator.TestValidate(candidate);

            result.ShouldNotHaveValidationErrorFor(c => c.Email);
            result.ShouldNotHaveValidationErrorFor(c => c.SecondaryEmail);
        }

        [Fact]
        public void Validate_DateOfBirthInFuture_HasError()
        {
            var candidate = new Candidate() { DateOfBirth = DateTime.UtcNow.AddDays(1) };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.DateOfBirth);
        }

        [Fact]
        public void Validate_NameIsTooLong_HasError()
        {
            var longName = new string('a', 257);
            var candidate = new Candidate() { FirstName = longName, LastName = longName };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.FirstName);
            result.ShouldHaveValidationErrorFor(c => c.LastName);
        }

        [Fact]
        public void Validate_TelephoneTooLong_HasError()
        {
            var longTelephone = new string('1', 26);
            var candidate = new Candidate()
            {
                AddressTelephone = longTelephone,
                MobileTelephone = longTelephone,
                Telephone = longTelephone,
                SecondaryTelephone = longTelephone
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.AddressTelephone);
            result.ShouldHaveValidationErrorFor(c => c.MobileTelephone);
            result.ShouldHaveValidationErrorFor(c => c.Telephone);
            result.ShouldHaveValidationErrorFor(c => c.SecondaryTelephone);
        }

        [Fact]
        public void Validate_TelephoneTooShort_HasError()
        {
            var shortTelephone = "1111";
            var candidate = new Candidate()
            {
                AddressTelephone = shortTelephone,
                MobileTelephone = shortTelephone,
                Telephone = shortTelephone,
                SecondaryTelephone = shortTelephone
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.AddressTelephone);
            result.ShouldHaveValidationErrorFor(c => c.MobileTelephone);
            result.ShouldHaveValidationErrorFor(c => c.Telephone);
            result.ShouldHaveValidationErrorFor(c => c.SecondaryTelephone);
        }

        [Theory]
        [InlineData("1234567", false)]
        [InlineData("123 4567", false)]
        [InlineData("  123 456 7", false)]
        [InlineData("+44 7503 483524", false)]
        [InlineData("+44 7503.483524", false)]
        [InlineData("+44 (7503) 483524", false)]
        [InlineData("abcgewgewgh", true)]
        [InlineData("abc2451215", true)]
        [InlineData("42154h53151", true)]
        [InlineData("5325.56fs326.32", true)]
        public void Validate_TelephoneFormat_ValidatesCorrectly(string telephone, bool hasError)
        {
            var candidate = new Candidate()
            {
                AddressTelephone = telephone,
                MobileTelephone = telephone,
                Telephone = telephone,
                SecondaryTelephone = telephone
            };
            var result = _validator.TestValidate(candidate);

            if (hasError)
            {
                result.ShouldHaveValidationErrorFor(c => c.AddressTelephone);
                result.ShouldHaveValidationErrorFor(c => c.MobileTelephone);
                result.ShouldHaveValidationErrorFor(c => c.Telephone);
                result.ShouldHaveValidationErrorFor(c => c.SecondaryTelephone);
            }
            else
            {
                result.ShouldNotHaveValidationErrorFor(c => c.AddressTelephone);
                result.ShouldNotHaveValidationErrorFor(c => c.MobileTelephone);
                result.ShouldNotHaveValidationErrorFor(c => c.Telephone);
                result.ShouldNotHaveValidationErrorFor(c => c.SecondaryTelephone);
            }
        }

        [Fact]
        public void Validate_AddressLineTooLong_HasError()
        {
            var longAddressLine = new string('a', 1025);
            var candidate = new Candidate()
            {
                AddressLine1 = longAddressLine,
                AddressLine2 = longAddressLine,
                AddressLine3 = longAddressLine,
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.AddressLine1);
            result.ShouldHaveValidationErrorFor(c => c.AddressLine2);
            result.ShouldHaveValidationErrorFor(c => c.AddressLine3);
        }

        [Fact]
        public void Validate_AddressCityIsTooLong_HasError()
        {
            var candidate = new Candidate() { AddressCity = new string('a', 129) };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.AddressCity);
        }

        [Fact]
        public void Validate_AddressStateOrProvinceIsTooLong_HasError()
        {
            var candidate = new Candidate() { AddressStateOrProvince = new string('a', 101) };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.AddressStateOrProvince);
        }

        [Theory]
        [InlineData("KY119YU", false)]
        [InlineData("KY11 9YU", false)]
        [InlineData("CA48LE", false)]
        [InlineData("CA4 8LE", false)]
        [InlineData("ky119yu", false)]
        [InlineData("KY999 9YU", true)]
        [InlineData("AZ1VS1", true)]
        public void Validate_AddressPostcodeFormat_ValidatesCorrectly(string postcode, bool hasError)
        {
            var candidate = new Candidate() { AddressPostcode = postcode };
            var result = _validator.TestValidate(candidate);

            if (hasError)
            {
                result.ShouldHaveValidationErrorFor(c => c.AddressPostcode);
            }
            else
            {
                result.ShouldNotHaveValidationErrorFor(c => c.AddressPostcode);
            }
        }

        [Fact]
        public void Validate_EligibilityRulesPassedIsNotTrueOrFalse_HasError()
        {
            var candidate = new Candidate() { EligibilityRulesPassed = "falsy" };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.EligibilityRulesPassed);
        }

        [Fact]
        public void Validate_IdFieldWithInvalidLookupItemId_HasError()
        {
            var candidate = new Candidate()
            {
                PreferredTeachingSubjectId = Guid.NewGuid(),
                SecondaryPreferredTeachingSubjectId = Guid.NewGuid(),
                CountryId = Guid.NewGuid(),
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.PreferredTeachingSubjectId);
            result.ShouldHaveValidationErrorFor(c => c.SecondaryPreferredTeachingSubjectId);
            result.ShouldHaveValidationErrorFor(c => c.CountryId);
        }

        [Fact]
        public void Validate_IdFieldWithInvalidPicklistItemId_HasError()
        {
            var candidate = new Candidate()
            {
                PreferredEducationPhaseId = 123,
                InitialTeacherTrainingYearId = 123,
                TypeId = 123,
                AssignmentStatusId = 123,
                MailingListSubscriptionChannelId = 123,
                EventsSubscriptionChannelId = 123,
                TeacherTrainingAdviserSubscriptionChannelId = 123,
                ChannelId = 123,
                AdviserEligibilityId = 123,
                AdviserRequirementId = 123,
                HasGcseMathsId = 123,
                HasGcseScienceId = 123,
                HasGcseEnglishId = 123,
                PlanningToRetakeGcseMathsId = 123,
                PlanningToRetakeGcseScienceId = 123,
                PlanningToRetakeGcseEnglishId = 123,
                ConsiderationJourneyStageId = 123,
                ApplyPhaseId = 123,
                ApplyStatusId = 123,
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor(c => c.PreferredEducationPhaseId);
            result.ShouldHaveValidationErrorFor(c => c.InitialTeacherTrainingYearId);
            result.ShouldHaveValidationErrorFor(c => c.TypeId);
            result.ShouldHaveValidationErrorFor(c => c.AssignmentStatusId);
            result.ShouldHaveValidationErrorFor(c => c.MailingListSubscriptionChannelId);
            result.ShouldHaveValidationErrorFor(c => c.EventsSubscriptionChannelId);
            result.ShouldHaveValidationErrorFor(c => c.TeacherTrainingAdviserSubscriptionChannelId);
            result.ShouldHaveValidationErrorFor(c => c.ChannelId);
            result.ShouldHaveValidationErrorFor(c => c.AdviserEligibilityId);
            result.ShouldHaveValidationErrorFor(c => c.AdviserRequirementId);
            result.ShouldHaveValidationErrorFor(c => c.HasGcseMathsId);
            result.ShouldHaveValidationErrorFor(c => c.HasGcseScienceId);
            result.ShouldHaveValidationErrorFor(c => c.HasGcseEnglishId);
            result.ShouldHaveValidationErrorFor(c => c.PlanningToRetakeGcseMathsId);
            result.ShouldHaveValidationErrorFor(c => c.PlanningToRetakeGcseScienceId);
            result.ShouldHaveValidationErrorFor(c => c.PlanningToRetakeGcseEnglishId);
            result.ShouldHaveValidationErrorFor(c => c.ConsiderationJourneyStageId);
            result.ShouldHaveValidationErrorFor(c => c.ApplyPhaseId);
            result.ShouldHaveValidationErrorFor(c => c.ApplyStatusId);
        }

        [Fact]
        public void Validate_ChannelIdIsNullWhenExistingCandidate_HasNoError()
        {
            var candidate = new Candidate() { Id = Guid.NewGuid(), ChannelId = null };
            var result = _validator.TestValidate(candidate);

            result.ShouldNotHaveValidationErrorFor("ChannelId");
        }

        [Fact]
        public void Validate_ChannelIdIsNullWhenNewCandidate_HasError()
        {
            var candidate = new Candidate() { Id = null, ChannelId = null };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor("ChannelId");
        }
    }
}
