using System;
using System.Collections.Generic;
using System.Linq;
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
            var mockEntityReference = new TypeEntity { Id = Guid.NewGuid().ToString() };
            var mockPickListItem = new TypeEntity { Id = "123" };
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };
            
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("dfe_teachingsubjectlist", null))
                .Returns(new[] { mockEntityReference }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("dfe_country", null))
                .Returns(new[] { mockEntityReference }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_preferrededucationphase01"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_ittyear"))
                .Returns(new[] { mockPickListItem }.AsQueryable);
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_channelcreation"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_gitismlservicesubscriptionchannel"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_gitiseventsservicesubscriptionchannel"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_websitehasgcseenglish"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_websiteplanningretakeenglishgcse"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_websitewhereinconsiderationjourney"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_typeofcandidate"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_candidatestatus"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_iscandidateeligibleforadviser"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_isadvisorrequiredos"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetPrivacyPolicies())
                .Returns(new[] { mockPrivacyPolicy }.AsQueryable());

            var candidate = new Candidate()
            {
                FirstName = "first",
                LastName = "last",
                Email = "email@candidate.com",
                DateOfBirth = DateTime.UtcNow.AddYears(-18),
                Telephone = "07584 734 576",
                AddressLine1 = "line1",
                AddressLine2 = "line2",
                AddressCity = "city",
                AddressPostcode = "KY119YU",
                HasGcseMathsId = int.Parse(mockPickListItem.Id),
                HasGcseEnglishId = int.Parse(mockPickListItem.Id),
                AdviserEligibilityId = int.Parse(mockPickListItem.Id),
                PlanningToRetakeGcseScienceId = int.Parse(mockPickListItem.Id),
                PlanningToRetakeGcseEnglishId = int.Parse(mockPickListItem.Id),
                TypeId = int.Parse(mockPickListItem.Id),
                AssignmentStatusId = int.Parse(mockPickListItem.Id),
                DoNotPostalMail = false,
                EligibilityRulesPassed = "true",
                ConsiderationJourneyStageId = int.Parse(mockPickListItem.Id),
                CountryId = Guid.Parse(mockEntityReference.Id),
                PreferredTeachingSubjectId = Guid.Parse(mockEntityReference.Id),
                PreferredEducationPhaseId = int.Parse(mockPickListItem.Id),
                InitialTeacherTrainingYearId = int.Parse(mockPickListItem.Id),
                ChannelId = int.Parse(mockPickListItem.Id),
                MailingListSubscriptionChannelId = int.Parse(mockPickListItem.Id),
                EventsSubscriptionChannelId = int.Parse(mockPickListItem.Id),
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id }
            };

            var result = _validator.TestValidate(candidate);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_TeachingEventRegistrationsWithDuplicateEvents_HasError()
        {
            var mockEvent = new TeachingEvent { Id = Guid.NewGuid() };

            var candidate = new Candidate
            {
                TeachingEventRegistrations = new List<TeachingEventRegistration>
                {
                    new TeachingEventRegistration {EventId = (Guid)mockEvent.Id},
                    new TeachingEventRegistration {EventId = (Guid)mockEvent.Id},
                }
            };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor("TeachingEventRegistrations");
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
                PhoneCall = new PhoneCall() { ScheduledAt = DateTime.UtcNow.AddDays(-10) }
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
        public void Validate_PrivacyPolicyIsNull_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.PrivacyPolicy, null as CandidatePrivacyPolicy);
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
        public void Validate_DateOfBirthIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.DateOfBirth, null as DateTime?);
        }

        [Fact]
        public void Validate_DateOfBirthInFuture_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.DateOfBirth, DateTime.UtcNow.AddDays(1));
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
        public void Validate_TelephoneIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.Telephone, null as string);
        }

        [Fact]
        public void Validate_TelephoneTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.Telephone, new string('1', 21));
        }

        [Fact]
        public void Validate_TelephoneTooShort_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.Telephone, new string('1', 4));
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
            if (hasError)
            {
                _validator.ShouldHaveValidationErrorFor(candidate => candidate.Telephone, telephone);
            }
            else
            {
                _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.Telephone, telephone);
            }
        }

        [Fact]
        public void Validate_AddressLine1IsEmpty_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.AddressLine1, null as string);
        }

        [Fact]
        public void Validate_AddressLine1IsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.AddressLine1, new string('a', 1025));
        }

        [Fact]
        public void Validate_AddressLine2IsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.AddressLine2, new string('a', 1025));
        }

        [Fact]
        public void Validate_AddressCityIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.AddressCity, null as string);
        }

        [Fact]
        public void Validate_AddressCityIsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.AddressCity, new string('a', 129));
        }

        [Fact]
        public void Validate_AddressPostcodeIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.AddressPostcode, null as string);
        }

        [Fact]
        public void Validate_AddressPostcodeIsTooLong_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.AddressPostcode, new string('a', 41));
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
            if (hasError)
            {
                _validator.ShouldHaveValidationErrorFor(candidate => candidate.AddressPostcode, postcode);
            }
            else
            {
                _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.AddressPostcode, postcode);
            }
        }

        [Fact]
        public void Validate_EligibilityRulesPassedIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.EligibilityRulesPassed, null as string);
        }

        [Fact]
        public void Validate_EligibilityRulesPassedIsNotTrueOrFalse_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.EligibilityRulesPassed, "falsy");
        }

        [Fact]
        public void Validate_PreferredTeachingSubjectIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.PreferredTeachingSubjectId, Guid.NewGuid());
        }

        [Fact]
        public void Validate_CountryIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.CountryId, null as Guid?);
        }

        [Fact]
        public void Validate_CountryIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.CountryId, Guid.NewGuid());
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
        public void Validate_InitialTeacherTrainingYearIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.InitialTeacherTrainingYearId, 123);
        }

        [Fact]
        public void Validate_InitialTeacherTrainingYearIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.InitialTeacherTrainingYearId, null as int?);
        }

        [Fact]
        public void Validate_TypeIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.TypeId, 123);
        }

        [Fact]
        public void Validate_TypeIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.TypeId, null as int?);
        }

        [Fact]
        public void Validate_AssignmentStatusIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.AssignmentStatusId, 123);
        }

        [Fact]
        public void Validate_AssignmentStatusIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.AssignmentStatusId, null as int?);
        }

        [Fact]
        public void Validate_MailingListSubscriptionChannelIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.MailingListSubscriptionChannelId, 123);
        }

        [Fact]
        public void Validate_EventSubscriptionChannelIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.EventsSubscriptionChannelId, 123);
        }

        [Fact]
        public void Validate_ChannelIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.ChannelId, 123);
        }

        [Fact]
        public void Validate_ChannelIdIsNullWhenExistingCandidate_HasNoError()
        {
            var candidate = new Candidate() { Id = Guid.NewGuid(), ChannelId = null};
            var result = _validator.TestValidate(candidate);

            result.ShouldNotHaveValidationErrorFor("ChannelId");
        }

        [Fact]
        public void Validate_ChannelIdIsNullWhenNewCandidate_HasError()
        {
            var candidate = new Candidate() { Id = null, ChannelId = null};
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor("ChannelId");
        }

        [Fact]
        public void Validate_ChannelIdIsNotNullWhenExistingCandidate_HasError()
        {
            var mockChannel = NewMock(123);
            _mockStore
                .Setup(mock => mock.GetTypeEntitites("contact", "dfe_channelcreation"))
                .Returns(new[] { mockChannel }.AsQueryable());
            var candidate = new Candidate() { Id = Guid.NewGuid(), ChannelId = int.Parse(mockChannel.Id) };
            var result = _validator.TestValidate(candidate);

            result.ShouldHaveValidationErrorFor("ChannelId");
        }

        [Fact]
        public void Validate_AdviserEligibilityIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.AdviserEligibilityId, 123);
        }

        [Fact]
        public void Validate_AdviserEligibilityIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.AdviserEligibilityId, null as int?);
        }

        [Fact]
        public void Validate_AdviserRequirementIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.AdviserRequirementId, 123);
        }

        [Fact]
        public void Validate_AdviserRequirementIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.AdviserRequirementId, null as int?);
        }

        [Fact]
        public void Validate_HasGcseMathsIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.HasGcseMathsId, 123);
        }

        [Fact]
        public void Validate_HasGcseMathsIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.HasGcseMathsId, null as int?);
        }

        [Fact]
        public void Validate_HasGcseScienceIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.HasGcseScienceId, 123);
        }

        [Fact]
        public void Validate_HasGcseScienceIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.HasGcseScienceId, null as int?);
        }

        [Fact]
        public void Validate_HasGcseEnglishIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.HasGcseEnglishId, 123);
        }

        [Fact]
        public void Validate_HasGcseEnglishIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.HasGcseEnglishId, null as int?);
        }

        [Fact]
        public void Validate_PlanningToRetakeGcseMathsIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.PlanningToRetakeGcseMathsId, 123);
        }

        [Fact]
        public void Validate_PlanningToRetakeGcseMathsIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.PlanningToRetakeGcseMathsId, null as int?);
        }

        [Fact]
        public void Validate_PlanningToRetakeGcseScienceIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.PlanningToRetakeGcseScienceId, 123);
        }

        [Fact]
        public void Validate_PlanningToRetakeGcseScienceIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.PlanningToRetakeGcseScienceId, null as int?);
        }

        [Fact]
        public void Validate_PlanningToRetakeGcseEnglishIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.PlanningToRetakeGcseEnglishId, 123);
        }

        [Fact]
        public void Validate_PlanningToRetakeGcseEnglishIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.PlanningToRetakeGcseEnglishId, null as int?);
        }

        [Fact]
        public void Validate_ConsiderationJourneyStageIdIsInvalid_HasError()
        {
            _validator.ShouldHaveValidationErrorFor(candidate => candidate.ConsiderationJourneyStageId, 123);
        }

        [Fact]
        public void Validate_ConsiderationJourneyStageIdIsNull_HasNoError()
        {
            _validator.ShouldNotHaveValidationErrorFor(candidate => candidate.ConsiderationJourneyStageId, null as int?);
        }

        private static TypeEntity NewMock(dynamic id)
        {
            return new TypeEntity { Id = id.ToString() };
        }
    }
}
