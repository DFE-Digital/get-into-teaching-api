using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.TeacherTrainingAdviser;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.TeacherTrainingAdviser
{
    public class TeacherTrainingAdviserSignUpTests
    {
        [Fact]
        public void Constructor_WithCandidate_MapsCorrectly()
        {
            var latestQualification = new CandidateQualification()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddDays(10),
                DegreeStatusId = 1,
                UkDegreeGradeId = 2,
                TypeId = 3,
                DegreeSubject = "English"
            };

            var qualifications = new List<CandidateQualification>()
            {
                new CandidateQualification() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(3) },
                latestQualification,
                new CandidateQualification() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(5) },
            };

            var latestPastTeachingPosition = new CandidatePastTeachingPosition()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddDays(10),
                SubjectTaughtId = Guid.NewGuid(),
            };

            var pastTeachingPositions = new List<CandidatePastTeachingPosition>()
            {
                new CandidatePastTeachingPosition() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(3) },
                latestPastTeachingPosition,
                new CandidatePastTeachingPosition() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(5) },
            };

            var candidate = new Candidate()
            {
                Id = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                CountryId = Guid.NewGuid(),
                InitialTeacherTrainingYearId = 1,
                PreferredEducationPhaseId = 2,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
                HasGcseEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                HasGcseMathsId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                HasGcseScienceId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                PlanningToRetakeGcseEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                PlanningToRetakeGcseMathsId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                PlanningToRetakeGcseScienceId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.UtcNow,
                AddressTelephone = "001234567",
                TeacherId = "abc123",
                AddressLine1 = "Address 1",
                AddressLine2 = "Address 2",
                AddressCity = "City",
                AddressPostcode = "KY11 9YU",
                Qualifications = qualifications,
                PastTeachingPositions = pastTeachingPositions,
                HasTeacherTrainingAdviserSubscription = true,
            };

            var response = new TeacherTrainingAdviserSignUp(candidate);

            response.CandidateId.Should().Be(candidate.Id);
            response.PreferredTeachingSubjectId.Should().Be(candidate.PreferredTeachingSubjectId);
            response.CountryId.Should().Be(candidate.CountryId);
            response.InitialTeacherTrainingYearId.Should().Be(candidate.InitialTeacherTrainingYearId);
            response.PreferredEducationPhaseId.Should().Be(candidate.PreferredEducationPhaseId);
            response.HasGcseMathsAndEnglishId.Should().Be((int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking);
            response.HasGcseScienceId.Should().Be(candidate.HasGcseScienceId);
            response.PlanningToRetakeGcseScienceId.Should().Be(candidate.PlanningToRetakeGcseScienceId);
            response.PlanningToRetakeGcseMathsAndEnglishId.Should().Be((int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking);
            response.TypeId.Should().Be((int)Candidate.Type.ReturningToTeacherTraining);
            response.Email.Should().Be(candidate.Email);
            response.FirstName.Should().Be(candidate.FirstName);
            response.LastName.Should().Be(candidate.LastName);
            response.TeacherId.Should().Be(candidate.TeacherId);
            response.AddressTelephone.Should().Be(candidate.AddressTelephone[2..]);
            response.AddressLine1.Should().Be(candidate.AddressLine1);
            response.AddressLine2.Should().Be(candidate.AddressLine2);
            response.AddressCity.Should().Be(candidate.AddressCity);
            response.AddressPostcode.Should().Be(candidate.AddressPostcode);

            response.QualificationId.Should().Be(latestQualification.Id);
            response.DegreeStatusId.Should().Be(latestQualification.DegreeStatusId);
            response.UkDegreeGradeId.Should().Be(latestQualification.UkDegreeGradeId);
            response.DegreeSubject.Should().Be(latestQualification.DegreeSubject);
            response.DegreeTypeId.Should().Be(latestQualification.TypeId);

            response.PastTeachingPositionId.Should().Be(latestPastTeachingPosition.Id);
            response.SubjectTaughtId.Should().Be(latestPastTeachingPosition.SubjectTaughtId);

            response.AlreadySubscribedToTeacherTrainingAdviser.Should().BeTrue();
            response.CanSubscribeToTeacherTrainingAdviser.Should().BeFalse();
        }

        [Fact]
        public void Candidate_MapsCorrectly()
        {
            var request = new TeacherTrainingAdviserSignUp()
            {
                CandidateId = Guid.NewGuid(),
                QualificationId = Guid.NewGuid(),
                SubjectTaughtId = Guid.NewGuid(),
                PastTeachingPositionId = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                CountryId = Guid.NewGuid(),
                AcceptedPolicyId = Guid.NewGuid(),
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
                UkDegreeGradeId = 0,
                DegreeStatusId = 1,
                DegreeTypeId = 2,
                InitialTeacherTrainingYearId = 3,
                PreferredEducationPhaseId = 4,
                HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                HasGcseScienceId = 7,
                PlanningToRetakeGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                PlanningToRetakeGcseScienceId = 9,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.UtcNow,
                AddressTelephone = "1234567",
                TeacherId = "abc123",
                DegreeSubject = "Maths",
                AddressLine1 = "Address 1",
                AddressLine2 = "Address 2",
                AddressCity = "City",
                AddressPostcode = "KY11 9YU",
                PhoneCallScheduledAt = DateTime.UtcNow,
            };

            var candidate = request.Candidate;

            candidate.Id.Should().Equals(request.CandidateId);
            candidate.PreferredTeachingSubjectId.Should().Equals(request.PreferredTeachingSubjectId);
            candidate.CountryId.Should().Equals(request.CountryId);
            candidate.InitialTeacherTrainingYearId.Should().Equals(request.InitialTeacherTrainingYearId);
            candidate.PreferredEducationPhaseId.Should().Equals(request.PreferredEducationPhaseId);
            candidate.HasGcseEnglishId.Should().Equals(request.HasGcseMathsAndEnglishId);
            candidate.HasGcseMathsId.Should().Equals(request.HasGcseMathsAndEnglishId);
            candidate.HasGcseScienceId.Should().Equals(request.HasGcseScienceId);
            candidate.PlanningToRetakeGcseEnglishId.Should().Equals(request.PlanningToRetakeGcseMathsAndEnglishId);
            candidate.PlanningToRetakeGcseMathsId.Should().Equals(request.PlanningToRetakeGcseMathsAndEnglishId);
            candidate.PlanningToRetakeGcseScienceId.Should().Equals(request.PlanningToRetakeGcseScienceId);
            candidate.AdviserRequirementId.Should().Be((int)Candidate.AdviserRequirement.Yes);
            candidate.AdviserEligibilityId.Should().Be((int)Candidate.AdviserEligibility.Yes);
            candidate.AssignmentStatusId.Should().Be((int)Candidate.AssignmentStatus.WaitingToBeAssigned);
            candidate.TypeId.Should().Be((int)Candidate.Type.ReturningToTeacherTraining);
            candidate.Email.Should().Be(request.Email);
            candidate.FirstName.Should().Be(request.FirstName);
            candidate.LastName.Should().Be(request.LastName);
            candidate.DateOfBirth.Should().Be(request.DateOfBirth);
            candidate.AddressPostcode.Should().Be(request.AddressPostcode);
            candidate.AddressTelephone.Should().Be("00" + request.AddressTelephone);
            candidate.TeacherId.Should().Be(request.TeacherId);
            candidate.AddressLine1.Should().Be(request.AddressLine1);
            candidate.AddressLine2.Should().Be(request.AddressLine2);
            candidate.AddressCity.Should().Be(request.AddressCity);
            candidate.AddressPostcode.Should().Be(request.AddressPostcode);
            candidate.ChannelId.Should().BeNull();
            candidate.EligibilityRulesPassed.Should().Be("true");
            candidate.OptOutOfSms.Should().BeFalse();
            candidate.DoNotBulkEmail.Should().BeTrue();
            candidate.DoNotEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeTrue();
            candidate.DoNotPostalMail.Should().BeTrue();
            candidate.DoNotSendMm.Should().BeTrue();
            candidate.PreferredPhoneNumberTypeId.Should().Be((int)Candidate.PhoneNumberType.Home);
            candidate.PreferredContactMethodId.Should().Be((int)Candidate.ContactMethod.Any);
            candidate.GdprConsentId.Should().Be((int)Candidate.GdprConsent.Consent);
            candidate.OptOutOfGdpr.Should().BeFalse();

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be((Guid)request.AcceptedPolicyId);
            candidate.PrivacyPolicy.AcceptedAt.Should().BeCloseTo(DateTime.UtcNow);

            candidate.PhoneCall.ScheduledAt.Should().Be((DateTime)request.PhoneCallScheduledAt);
            candidate.PhoneCall.Telephone.Should().Be("00" + request.AddressTelephone);
            candidate.PhoneCall.ChannelId.Should().Be((int)PhoneCall.Channel.CallbackRequest);
            candidate.PhoneCall.DestinationId.Should().Be((int)PhoneCall.Destination.International);
            candidate.PhoneCall.Subject.Should().Be("Scheduled phone call requested by John Doe");

            candidate.PastTeachingPositions.First().Id.Should().Be(request.PastTeachingPositionId);
            candidate.PastTeachingPositions.First().SubjectTaughtId.Should().Be(request.SubjectTaughtId);
            candidate.PastTeachingPositions.First().EducationPhaseId.Should().Be((int)CandidatePastTeachingPosition.EducationPhase.Secondary);

            candidate.Qualifications.First().Id.Should().Be(request.QualificationId);
            candidate.Qualifications.First().UkDegreeGradeId.Should().Be(request.UkDegreeGradeId);
            candidate.Qualifications.First().DegreeStatusId.Should().Be(request.DegreeStatusId);
            candidate.Qualifications.First().DegreeSubject.Should().Be(request.DegreeSubject);
            candidate.Qualifications.First().TypeId.Should().Be(request.DegreeTypeId);

            candidate.HasTeacherTrainingAdviserSubscription.Should().BeTrue();
        }

        [Fact]
        public void Candidate_EducationPhaseIsPrimary_SetsPreferredTeachingSubjectIdToPrimary()
        {
            var request = new TeacherTrainingAdviserSignUp()
            {
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Primary,
            };

            var candidate = request.Candidate;

            candidate.PreferredTeachingSubjectId.Should().Be(LookupItem.PrimaryTeachingSubjectId);
        }

        [Fact]
        public void Candidate_ReturningToTeaching_CorrectConsent()
        {
            var request = new TeacherTrainingAdviserSignUp() { TypeId = (int)Candidate.Type.ReturningToTeacherTraining };

            var candidate = request.Candidate;

            candidate.DoNotBulkEmail.Should().BeTrue();
            candidate.DoNotSendMm.Should().BeTrue();
        }

        [Fact]
        public void Candidate_InterestedInTeaching_CorrectConsent()
        {
            var request = new TeacherTrainingAdviserSignUp() { TypeId = (int)Candidate.Type.InterestedInTeacherTraining };

            var candidate = request.Candidate;

            candidate.DoNotBulkEmail.Should().BeFalse();
            candidate.DoNotSendMm.Should().BeFalse();
        }

        [Fact]
        public void Candidate_GcseIdIsNull_DefaultsToNotAnswered()
        {
            var request = new TeacherTrainingAdviserSignUp()
            {
                HasGcseMathsAndEnglishId = null,
                HasGcseScienceId = null,
                PlanningToRetakeGcseMathsAndEnglishId = null,
                PlanningToRetakeGcseScienceId = null,
            };

            var candidate = request.Candidate;

            var gcses = new int?[]
            {
                candidate.HasGcseEnglishId,
                candidate.HasGcseMathsId,
                candidate.HasGcseScienceId,
                candidate.PlanningToRetakeGcseEnglishId,
                candidate.PlanningToRetakeGcseMathsId,
                candidate.PlanningToRetakeGcseScienceId
            };

            gcses.Should().AllBeEquivalentTo((int)Candidate.GcseStatus.NotAnswered);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsTeacherTrainingAdviser()
        {
            var request = new TeacherTrainingAdviserSignUp() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.TeacherTrainingAdviser);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNotNull_IsNotChanged()
        {
            var request = new TeacherTrainingAdviserSignUp() { CandidateId = Guid.NewGuid() };

            request.Candidate.ChannelId.Should().BeNull();
            request.Candidate.ChangedPropertyNames.Should().NotContain("ChannelId");
        }

        [Fact]
        public void Candidate_PhoneCallScheduledAtIsNull_NoPhoneCallIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { PhoneCallScheduledAt = null };

            request.Candidate.PhoneCall.Should().BeNull();
        }

        [Fact]
        public void Candidate_SubjectTaughtIdIsNull_NoPastTeachingPositionIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { SubjectTaughtId = null };

            request.Candidate.PastTeachingPositions.Should().BeEmpty();
        }

        [Fact]
        public void Candidate_QualificationFieldsAreNull_NoQualificationIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { UkDegreeGradeId = null, DegreeStatusId = null, DegreeSubject = null, DegreeTypeId = null };

            request.Candidate.Qualifications.Should().BeEmpty();
        }

        [Fact]
        public void Candidate_UkDegreeGradeIdIsPresent_QualificationIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { UkDegreeGradeId = 1, DegreeStatusId = null, DegreeSubject = null, DegreeTypeId = null };

            request.Candidate.Qualifications.Count.Should().Be(1);
        }

        [Fact]
        public void Candidate_DegreeStatusIdIsPresent_QualificationIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { UkDegreeGradeId = null, DegreeStatusId = 1, DegreeSubject = null, DegreeTypeId = null };

            request.Candidate.Qualifications.Count.Should().Be(1);
        }

        [Fact]
        public void Candidate_DegreeTypeIdIsPresent_QualificationIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { UkDegreeGradeId = null, DegreeStatusId = null, DegreeSubject = null, DegreeTypeId = 1 };

            request.Candidate.Qualifications.Count.Should().Be(1);
        }

        [Fact]
        public void Candidate_DegreeSubjectIdIsPresent_QualificationIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { UkDegreeGradeId = null, DegreeStatusId = null, DegreeSubject = "Maths", DegreeTypeId = null };

            request.Candidate.Qualifications.Count.Should().Be(1);
        }

        [Fact]
        public void Candidate_UkCountry_PhoneCallDestinationIsCorrect()
        {
            var request = new TeacherTrainingAdviserSignUp() { CountryId = LookupItem.UnitedKingdomCountryId, AddressTelephone = "123456789", PhoneCallScheduledAt = DateTime.UtcNow };

            request.Candidate.PhoneCall.DestinationId.Should().Be((int)PhoneCall.Destination.Uk);
        }

        [Fact]
        public void Candidate_OverseasCountry_PhoneCallDestinationIsCorrect()
        {
            var request = new TeacherTrainingAdviserSignUp() { CountryId = Guid.NewGuid(), AddressTelephone = "123456789", PhoneCallScheduledAt = DateTime.UtcNow };

            request.Candidate.PhoneCall.DestinationId.Should().Be((int)PhoneCall.Destination.International);
        }

        [Fact]
        public void Candidate_NullTelephone_PhoneCallDestinationIsCorrect()
        {
            var request = new TeacherTrainingAdviserSignUp() { CountryId = Guid.NewGuid(), AddressTelephone = null, PhoneCallScheduledAt = DateTime.UtcNow };

            request.Candidate.PhoneCall.DestinationId.Should().BeNull();
        }

        [Fact]
        public void Candidate_ReturningToTeaching_PreferredEducationPhaseIdDefaultsToSecondary()
        {
            var request = new TeacherTrainingAdviserSignUp() { TypeId = (int)Candidate.Type.ReturningToTeacherTraining };

            request.Candidate.PreferredEducationPhaseId.Should().Be((int)Candidate.PreferredEducationPhase.Secondary);
        }

        [Fact]
        public void Candidate_PhoneCallScheduledAtIsNull_EligibilityRulesPassedIsFalse()
        {
            var request = new TeacherTrainingAdviserSignUp() { PhoneCallScheduledAt = null };

            request.Candidate.EligibilityRulesPassed.Should().Be("false");
        }

        [Fact]
        public void Candidate_DegreeTypeDegree_IsEligibleForAdviser()
        {
            var request = new TeacherTrainingAdviserSignUp() { DegreeTypeId = (int)CandidateQualification.DegreeType.Degree };

            request.Candidate.AssignmentStatusId.Should().Be((int)Candidate.AssignmentStatus.WaitingToBeAssigned);
            request.Candidate.AdviserEligibilityId.Should().Be((int)Candidate.AdviserEligibility.Yes);
            request.Candidate.AdviserRequirementId.Should().Be((int)Candidate.AdviserRequirement.Yes);
        }

        [Fact]
        public void Candidate_DegreeTypeDegreeEquivalent_IsNotEligibleForAdviser()
        {
            var request = new TeacherTrainingAdviserSignUp() { DegreeTypeId = (int)CandidateQualification.DegreeType.DegreeEquivalent };

            request.Candidate.AssignmentStatusId.Should().BeNull();
            request.Candidate.AdviserEligibilityId.Should().BeNull();
            request.Candidate.AdviserRequirementId.Should().BeNull();
        }

        [Fact]
        public void Candidate_ReturningToTeaching_IsEligibleForAdviser()
        {
            var request = new TeacherTrainingAdviserSignUp() { TypeId = (int)Candidate.Type.ReturningToTeacherTraining };

            request.Candidate.IsReturningToTeaching().Should().BeTrue();
            request.Candidate.AssignmentStatusId.Should().Be((int)Candidate.AssignmentStatus.WaitingToBeAssigned);
            request.Candidate.AdviserEligibilityId.Should().Be((int)Candidate.AdviserEligibility.Yes);
            request.Candidate.AdviserRequirementId.Should().Be((int)Candidate.AdviserRequirement.Yes);
            request.Candidate.StatusIsWaitingToBeAssignedAt.Should().BeCloseTo(DateTime.UtcNow);
        }

        [Fact]
        public void Candidate_InterestedInTeaching_IsNotEligibleForAdviser()
        {
            var request = new TeacherTrainingAdviserSignUp() { TypeId = (int)Candidate.Type.InterestedInTeacherTraining };

            request.Candidate.IsReturningToTeaching().Should().BeFalse();
            request.Candidate.AssignmentStatusId.Should().BeNull();
            request.Candidate.AdviserEligibilityId.Should().BeNull();
            request.Candidate.AdviserRequirementId.Should().BeNull();
            request.Candidate.StatusIsWaitingToBeAssignedAt.Should().BeNull();
        }

        [Fact]
        public void Candidate_AddressPostcode_IsFormatted()
        {
            var request = new TeacherTrainingAdviserSignUp() { AddressPostcode = "ky119yu" };

            request.Candidate.AddressPostcode.Should().Be("KY11 9YU");
        }

        [Theory]
        [InlineData("(65).234.543.435", "0065234543435")]
        [InlineData("+818495394", "00818495394")]
        [InlineData("+(81) 849 5394", "00818495394")]
        [InlineData("+44756483443", "0756483443")]
        [InlineData("+440756483443", "0756483443")]
        public void PhoneCallScheduledAt_InternationalCallback_IsSanitized(string input, string expected)
        {
            var request = new TeacherTrainingAdviserSignUp()
            {
                AddressTelephone = input,
                CountryId = Guid.NewGuid(),
                PhoneCallScheduledAt = DateTime.UtcNow
            };

            request.Candidate.PhoneCall.Telephone.Should().Be(expected);
        }

        [Theory]
        [InlineData(true, (int)TeacherTrainingAdviserSignUp.ResubscribableAdviserStatus.NoLongerPursuingTeaching, true)]
        [InlineData(false, -12345, true)]
        [InlineData(true, null, false)]
        [InlineData(false, null, true)]
        [InlineData(true, -12345, false)]
        public void CanSubscribeToTeacherTrainingAdviser_ReturnsCorrectly(bool hasAdviser, int? adviserStatus, bool expected)
        {
            var candidate = new Candidate() {
                HasTeacherTrainingAdviserSubscription = hasAdviser,
                AdviserStatus = adviserStatus
            };

            var response = new TeacherTrainingAdviserSignUp(candidate);

            response.CanSubscribeToTeacherTrainingAdviser.Should().Be(expected);
        }
    }
}
