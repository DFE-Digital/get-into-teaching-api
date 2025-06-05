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
                AdviserStatusId = (int)TeacherTrainingAdviserSignUp.ResubscribableAdviserStatus.AcceptedIttOffer,
                AssignmentStatusId = (int)Candidate.AssignmentStatus.WaitingToBeAssigned,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.UtcNow,
                AddressTelephone = "001234567",
                TeacherId = "abc123",
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
            response.AdviserStatusId.Should().Be((int)TeacherTrainingAdviserSignUp.ResubscribableAdviserStatus.AcceptedIttOffer);
            response.AssignmentStatusId.Should().Be(candidate.AssignmentStatusId);
            response.Email.Should().Be(candidate.Email);
            response.FirstName.Should().Be(candidate.FirstName);
            response.LastName.Should().Be(candidate.LastName);
            response.TeacherId.Should().Be(candidate.TeacherId);
            response.AddressTelephone.Should().Be(candidate.AddressTelephone[2..]);
            response.AddressPostcode.Should().Be(candidate.AddressPostcode);

            response.QualificationId.Should().Be(latestQualification.Id);
            response.DegreeStatusId.Should().Be(latestQualification.DegreeStatusId);
            response.UkDegreeGradeId.Should().Be(latestQualification.UkDegreeGradeId);
            response.DegreeSubject.Should().Be(latestQualification.DegreeSubject);
            response.DegreeTypeId.Should().Be(latestQualification.TypeId);

            response.PastTeachingPositionId.Should().Be(latestPastTeachingPosition.Id);
            response.SubjectTaughtId.Should().Be(latestPastTeachingPosition.SubjectTaughtId);

            response.CanSubscribeToTeacherTrainingAdviser.Should().BeTrue();
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
                DegreeTypeId = (int)CandidateQualification.DegreeType.Degree,
                InitialTeacherTrainingYearId = 3,
                PreferredEducationPhaseId = (int)Candidate.PreferredEducationPhase.Secondary,
                HasGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                HasGcseScienceId = 7,
                PlanningToRetakeGcseMathsAndEnglishId = (int)Candidate.GcseStatus.HasOrIsPlanningOnRetaking,
                PlanningToRetakeGcseScienceId = 9,
                AdviserStatusId = null,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.UtcNow,
                AddressTelephone = "1234567",
                TeacherId = "abc123",
                DegreeSubject = "Maths",
                AddressPostcode = "KY11 9YU",
                PhoneCallScheduledAt = DateTime.UtcNow,
                CreationChannelSourceId = 222750003,
                CreationChannelServiceId = 222750002,
                CreationChannelActivityId = 222750001,
            };

            var candidate = request.Candidate;

            candidate.Id.Should().Be(request.CandidateId);
            candidate.PreferredTeachingSubjectId.Should().Be(request.PreferredTeachingSubjectId);
            candidate.CountryId.Should().Be(request.CountryId);
            candidate.InitialTeacherTrainingYearId.Should().Be(request.InitialTeacherTrainingYearId);
            candidate.PreferredEducationPhaseId.Should().Be(request.PreferredEducationPhaseId);
            candidate.HasGcseEnglishId.Should().Be(request.HasGcseMathsAndEnglishId);
            candidate.HasGcseMathsId.Should().Be(request.HasGcseMathsAndEnglishId);
            candidate.HasGcseScienceId.Should().Be(request.HasGcseScienceId);
            candidate.PlanningToRetakeGcseEnglishId.Should().Be(request.PlanningToRetakeGcseMathsAndEnglishId);
            candidate.PlanningToRetakeGcseMathsId.Should().Be(request.PlanningToRetakeGcseMathsAndEnglishId);
            candidate.PlanningToRetakeGcseScienceId.Should().Be(request.PlanningToRetakeGcseScienceId);
            candidate.AdviserStatusId.Should().BeNull();
            candidate.AdviserRequirementId.Should().BeNull();
            candidate.AdviserEligibilityId.Should().BeNull();
            candidate.AssignmentStatusId.Should().BeNull();
            candidate.TypeId.Should().Be((int)Candidate.Type.ReturningToTeacherTraining);
            candidate.Email.Should().Be(request.Email);
            candidate.FirstName.Should().Be(request.FirstName);
            candidate.LastName.Should().Be(request.LastName);
            candidate.DateOfBirth.Should().Be(request.DateOfBirth);
            candidate.AddressPostcode.Should().Be(request.AddressPostcode);
            candidate.AddressTelephone.Should().Be("00" + request.AddressTelephone);
            candidate.TeacherId.Should().Be(request.TeacherId);
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

            candidate.RegistrationStatusId.Should().BeNull();

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be((Guid)request.AcceptedPolicyId);
            candidate.PrivacyPolicy.AcceptedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));

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

            candidate.ContactChannelCreations.First().CreationChannelSourceId.Should().Be(request.CreationChannelSourceId);
            candidate.ContactChannelCreations.First().CreationChannelServiceId.Should().Be(request.CreationChannelServiceId);
            candidate.ContactChannelCreations.First().CreationChannelActivityId.Should().Be(request.CreationChannelActivityId);

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

            candidate.PreferredTeachingSubjectId.Should().Be(TeachingSubject.PrimaryTeachingSubjectId);
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
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsTeacherTrainingAdviser_WithoutDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "1");
            
            var request = new TeacherTrainingAdviserSignUp() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.TeacherTrainingAdviser);
            
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
        }
        
        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsTeacherTrainingAdviser_WithDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "0");
            
            var request = new TeacherTrainingAdviserSignUp() { CandidateId = null };
            
            request.Candidate.ChannelId.Should().Be(null);
            
            var ccc = request.Candidate.ContactChannelCreations.First();
            ccc.CreationChannel.Should().Be(true);
            ccc.CreationChannelSourceId.Should().Be((int?)ContactChannelCreation.CreationChannelSource.GITWebsite);
            ccc.CreationChannelServiceId.Should().Be((int?)ContactChannelCreation.CreationChannelService.TeacherTrainingAdviserService);
            ccc.CreationChannelActivityId.Should().Be(null);
            
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
        }


        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNotNull_IsNotChanged()
        {
            var request = new TeacherTrainingAdviserSignUp() { CandidateId = Guid.NewGuid() };

            request.Candidate.ChannelId.Should().BeNull();
            request.Candidate.ChangedPropertyNames.Should().NotContain("ChannelId");
        }
        
        [Fact]
        public void Candidate_ContactChannelCreationWhenCandidateIdIsNull()
        {
            var request = new TeacherTrainingAdviserSignUp()
            {
                CandidateId = null, 
                CreationChannelSourceId = 222750000,
                CreationChannelServiceId = 222750001,
                CreationChannelActivityId = 222750002,
            };

            request.Candidate.ChannelId.Should().BeNull();
            request.Candidate.ChangedPropertyNames.Should().Contain("ContactChannelCreations");
            request.Candidate.ContactChannelCreations.First().CreationChannelSourceId.Should().Be(222750000);
            request.Candidate.ContactChannelCreations.First().CreationChannelServiceId.Should().Be(222750001);
            request.Candidate.ContactChannelCreations.First().CreationChannelActivityId.Should().Be(222750002);
        }
        
        [Fact]
        public void Candidate_ContactChannelCreationWhenCandidateIdIsNotNull()
        {
            var request = new TeacherTrainingAdviserSignUp()
            {
                CandidateId = Guid.NewGuid(), 
                CreationChannelSourceId = 222750000,
                CreationChannelServiceId = 222750001,
                CreationChannelActivityId = 222750002,
            };

            request.Candidate.ChannelId.Should().BeNull();
            request.Candidate.ChangedPropertyNames.Should().NotContain("ChannelId");
            request.Candidate.ChangedPropertyNames.Should().Contain("ContactChannelCreations");
            request.Candidate.ContactChannelCreations.First().CreationChannelSourceId.Should().Be(222750000);
            request.Candidate.ContactChannelCreations.First().CreationChannelServiceId.Should().Be(222750001);
            request.Candidate.ContactChannelCreations.First().CreationChannelActivityId.Should().Be(222750002);
        }

        [Fact]
        public void Candidate_WhenChannelIsProvided_SetsOnAllModels_WithoutDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "1");
            
            var request = new TeacherTrainingAdviserSignUp() { ChannelId = 123 };

            request.Candidate.ChannelId.Should().Be(123);
            request.Candidate.TeacherTrainingAdviserSubscriptionChannelId.Should().Be(123);
            
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
        }
        
        [Fact]
        public void Candidate_WhenChannelIsProvided_SetsOnAllModels_WithDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "0");
            
            var request = new TeacherTrainingAdviserSignUp() { ChannelId = 123 };
            
            request.Candidate.ChannelId.Should().Be(null);
            
            var ccc = request.Candidate.ContactChannelCreations.First();
            ccc.CreationChannel.Should().Be(true);
            ccc.CreationChannelSourceId.Should().Be((int?)ContactChannelCreation.CreationChannelSource.GITWebsite);
            ccc.CreationChannelServiceId.Should().Be((int?)ContactChannelCreation.CreationChannelService.TeacherTrainingAdviserService);
            ccc.CreationChannelActivityId.Should().Be(null);
            
            request.Candidate.TeacherTrainingAdviserSubscriptionChannelId.Should().Be(123);
            
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
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
            var request = new TeacherTrainingAdviserSignUp() { CountryId = Country.UnitedKingdomCountryId, AddressTelephone = "123456789", PhoneCallScheduledAt = DateTime.UtcNow };

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
        public void Candidate_PhoneCallScheduled_IsNotEligibleForAdviser()
        {
            var request = new TeacherTrainingAdviserSignUp() { PhoneCallScheduledAt = DateTime.UtcNow };

            request.Candidate.StatusIsWaitingToBeAssignedAt.Should().BeNull();
            request.Candidate.AssignmentStatusId.Should().BeNull();
            request.Candidate.AdviserEligibilityId.Should().BeNull();
            request.Candidate.AdviserRequirementId.Should().BeNull();
        }

        [Fact]
        public void Candidate_PhoneCallNotScheduled_IsEligibleForAdviser()
        {
            var request = new TeacherTrainingAdviserSignUp() { PhoneCallScheduledAt = null };


            request.Candidate.AssignmentStatusId.Should().Be((int)Candidate.AssignmentStatus.WaitingToBeAssigned);
            request.Candidate.AdviserEligibilityId.Should().Be((int)Candidate.AdviserEligibility.Yes);
            request.Candidate.AdviserRequirementId.Should().Be((int)Candidate.AdviserRequirement.Yes);
            request.Candidate.StatusIsWaitingToBeAssignedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
        }

        [Fact]
        public void Candidate_AddressPostcode_IsFormatted()
        {
            var request = new TeacherTrainingAdviserSignUp() { AddressPostcode = "ky119yu" };

            request.Candidate.AddressPostcode.Should().Be("KY11 9YU");
        }

        [Fact]
        public void Candidate_WithClosedAdviserStatusId_UpdatesAssignmentAndRegistrationStatus()
        {
            var request = new TeacherTrainingAdviserSignUp() { AdviserStatusId = (int)TeacherTrainingAdviserSignUp.ResubscribableAdviserStatus.AlreadyHasQts };

            request.Candidate.AssignmentStatusId.Should().Be((int)Candidate.AssignmentStatus.WaitingToBeAssigned);
            request.Candidate.RegistrationStatusId.Should().Be((int)Candidate.RegistrationStatus.ReRegistered);
            request.Candidate.StatusIsWaitingToBeAssignedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
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
        public void CanSubscribeToTeacherTrainingAdviser_ReturnsCorrectly(bool hasAdviser, int? adviserStatusId, bool expected)
        {
            var candidate = new Candidate() {
                HasTeacherTrainingAdviserSubscription = hasAdviser,
                AdviserStatusId = adviserStatusId
            };

            var response = new TeacherTrainingAdviserSignUp(candidate);

            response.CanSubscribeToTeacherTrainingAdviser.Should().Be(expected);
        }
    }
}
