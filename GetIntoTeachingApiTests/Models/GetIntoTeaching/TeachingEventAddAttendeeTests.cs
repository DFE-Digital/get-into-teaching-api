using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Xunit;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching
{
    public class TeachingEventAddAttendeeTests
    {
        [Fact]
        public void Constructor_WithExistingCandidate_MapsCorrectly()
        {
            var latestQualification = new CandidateQualification()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddDays(10),
                UkDegreeGradeId = 1,
            };

            var qualifications = new List<CandidateQualification>()
            {
                new CandidateQualification() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(3) },
                latestQualification,
                new CandidateQualification() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(5) },
            };

            var candidate = new Candidate()
            {
                Id = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                ConsiderationJourneyStageId = 1,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                AddressTelephone = "001234567",
                AddressPostcode = "KY11 9YU",
                Qualifications = qualifications,
                HasEventsSubscription = true,
                HasTeacherTrainingAdviserSubscription = true,
            };

            var response = new TeachingEventAddAttendee(candidate);

            response.CandidateId.Should().Be(candidate.Id);
            response.PreferredTeachingSubjectId.Should().Be(candidate.PreferredTeachingSubjectId);
            response.ConsiderationJourneyStageId.Should().Be(candidate.ConsiderationJourneyStageId);
            response.Email.Should().Be(candidate.Email);
            response.FirstName.Should().Be(candidate.FirstName);
            response.LastName.Should().Be(candidate.LastName);
            response.AddressTelephone.Should().Be(candidate.AddressTelephone[2..]);
            response.AddressPostcode.Should().Be(candidate.AddressPostcode);

            response.QualificationId.Should().Be(latestQualification.Id);
            response.DegreeStatusId.Should().Be(latestQualification.DegreeStatusId);

            response.AlreadySubscribedToEvents.Should().BeTrue();
            response.AlreadySubscribedToMailingList.Should().BeFalse();
            response.AlreadySubscribedToTeacherTrainingAdviser.Should().BeTrue();

            response.IsVerified.Should().BeTrue();
        }

        [Fact]
        public void ExistingCandidate_MapsCorrectly()
        {
            var request = new TeachingEventAddAttendee()
            {
                EventId = Guid.NewGuid(),
                CandidateId = Guid.NewGuid(),
                QualificationId = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                AcceptedPolicyId = Guid.NewGuid(),
                DegreeStatusId = 1,
                ConsiderationJourneyStageId = 1,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                AddressTelephone = "1234567",
                AddressPostcode = "KY11 9YU",
                SubscribeToMailingList = true,
                IsWalkIn = false,
                CreationChannelSourceId = 222750003,
                CreationChannelServiceId = 222750002,
                CreationChannelActivityId = 222750001,
                AccessibilityNeedsForEvent = "test for AccessibilityNeedsForEvent"
            };

            var candidate = request.Candidate;

            candidate.Id.Should().Be(request.CandidateId);
            candidate.ConsiderationJourneyStageId.Should().Be(request.ConsiderationJourneyStageId);
            candidate.PreferredTeachingSubjectId.Should().Be(request.PreferredTeachingSubjectId);

            candidate.Email.Should().Be(request.Email);
            candidate.FirstName.Should().Be(request.FirstName);
            candidate.LastName.Should().Be(request.LastName);
            candidate.AddressPostcode.Should().Be(request.AddressPostcode);
            candidate.AddressTelephone.Should().Be(request.AddressTelephone);
            candidate.ChannelId.Should().BeNull();
            candidate.OptOutOfSms.Should().BeFalse();
            candidate.DoNotBulkEmail.Should().BeFalse();
            candidate.DoNotEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeTrue();
            candidate.DoNotPostalMail.Should().BeTrue();
            candidate.DoNotSendMm.Should().BeFalse();
            candidate.PreferredPhoneNumberTypeId.Should().Be((int)Candidate.PhoneNumberType.Home);
            candidate.PreferredContactMethodId.Should().Be((int)Candidate.ContactMethod.Any);
            candidate.GdprConsentId.Should().Be((int)Candidate.GdprConsent.Consent);
            candidate.OptOutOfGdpr.Should().BeFalse();

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be((Guid)request.AcceptedPolicyId);
            candidate.PrivacyPolicy.AcceptedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));

            candidate.HasEventsSubscription.Should().BeTrue();
            candidate.HasMailingListSubscription.Should().BeTrue();

            candidate.TeachingEventRegistrations.First().EventId.Should().Be((Guid)request.EventId);
            candidate.TeachingEventRegistrations.First().ChannelId.Should().Be((int)TeachingEventRegistration.Channel.Event);
            candidate.TeachingEventRegistrations.First().IsCancelled.Should().BeFalse();
            candidate.TeachingEventRegistrations.First().RegistrationNotificationSeen.Should().BeFalse();
            candidate.TeachingEventRegistrations.First().AccessibilityNeedsForEvent.Should().Be(request.AccessibilityNeedsForEvent);

            candidate.Qualifications.First().DegreeStatusId.Should().Be(request.DegreeStatusId);
            candidate.Qualifications.First().TypeId.Should().Be((int)CandidateQualification.DegreeType.Degree);
            candidate.Qualifications.First().Id.Should().Be(request.QualificationId);
            
            var contactChannelCreation = candidate.ContactChannelCreations.First();
            contactChannelCreation.CreationChannel.Should().Be(false);
            contactChannelCreation.CreationChannelSourceId.Should().Be(request.CreationChannelSourceId);
            contactChannelCreation.CreationChannelServiceId.Should().Be(request.CreationChannelServiceId);
            contactChannelCreation.CreationChannelActivityId.Should().Be(request.CreationChannelActivityId);
            candidate.ChannelId.Should().Be(null);
        }
        
        [Fact]
        public void NewCandidate_MapsCorrectly()
        {
            var request = new TeachingEventAddAttendee()
            {
                CandidateId = null,
                CreationChannelSourceId = 222750003,
                CreationChannelServiceId = 222750002,
                CreationChannelActivityId = 222750001,
            };
            
            var contactChannelCreation = request.Candidate.ContactChannelCreations.First();
            contactChannelCreation.CreationChannel.Should().Be(true);
            contactChannelCreation.CreationChannelSourceId.Should().Be(request.CreationChannelSourceId);
            contactChannelCreation.CreationChannelServiceId.Should().Be(request.CreationChannelServiceId);
            contactChannelCreation.CreationChannelActivityId.Should().Be(request.CreationChannelActivityId);
            request.Candidate.ChannelId.Should().Be(null);
        }

        [Fact]
        public void Candidate_OptionalAttributesWhenNull_DoesNotSet()
        {
            var request = new TeachingEventAddAttendee()
            {
                ConsiderationJourneyStageId = null,
                PreferredTeachingSubjectId = null,
                AddressTelephone = null,
                AddressPostcode = null,
            };

            request.Candidate.ChangedPropertyNames.Should().NotContain("ConsiderationJourneyStageId");
            request.Candidate.ChangedPropertyNames.Should().NotContain("PreferredTeachingSubjectId");
            request.Candidate.ChangedPropertyNames.Should().NotContain("AddressTelephone");
            request.Candidate.ChangedPropertyNames.Should().NotContain("AddressPostcode");
        }

        [Fact]
        public void Candidate_SubscribeToMailingListIsFalse_ConsentIsCorrect()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = false, AddressPostcode = null };

            request.Candidate.DoNotSendMm.Should().BeTrue();
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsMailingList_WithoutDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "1");
            
            var request = new TeachingEventAddAttendee() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.Event);
            
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
        }
        
        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsMailingList_WithDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "0");
            
            var request = new TeachingEventAddAttendee() { CandidateId = null };
            request.Candidate.ChannelId.Should().Be(null);
            
            var ccc = request.Candidate.ContactChannelCreations.First();
            ccc.CreationChannel.Should().Be(true);
            ccc.CreationChannelSourceId.Should().Be((int?)ContactChannelCreation.CreationChannelSource.GITWebsite);
            ccc.CreationChannelServiceId.Should().Be((int?)ContactChannelCreation.CreationChannelService.Events);
            ccc.CreationChannelActivityId.Should().Be(null);
            
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
        }

        [Fact]
        public void Candidate_WithNonNullChannelIdWhenCandidateIdIsNull_IsMailingList_WithoutDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "1");
            
            var request = new TeachingEventAddAttendee() { CandidateId = null, ChannelId = 456 };

            request.Candidate.ChannelId.Should().Be(456);
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
        }
        
        [Fact]
        public void Candidate_WithNonNullChannelIdWhenCandidateIdIsNull_IsMailingList_WithDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "0");
            
            var request = new TeachingEventAddAttendee() { CandidateId = null, ChannelId = 456 };

            request.Candidate.ChannelId.Should().Be(null);
            
            var ccc = request.Candidate.ContactChannelCreations.First();
            ccc.CreationChannel.Should().Be(true);
            ccc.CreationChannelSourceId.Should().Be((int?)ContactChannelCreation.CreationChannelSource.GITWebsite);
            ccc.CreationChannelServiceId.Should().Be((int?)ContactChannelCreation.CreationChannelService.Events);
            ccc.CreationChannelActivityId.Should().Be(null);
            
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNotNull_IsNotChanged()
        {
            var request = new TeachingEventAddAttendee() { CandidateId = Guid.NewGuid() };

            request.Candidate.ChannelId.Should().BeNull();
            request.Candidate.ChangedPropertyNames.Should().NotContain("ChannelId");
        }

        [Fact]
        public void Candidate_DegreeStatusIdIsNull_DoesNotCreateQualification()
        {
            var request = new TeachingEventAddAttendee() { DegreeStatusId = null };

            request.Candidate.Qualifications.Should().BeEmpty();
        }

        [Fact]
        public void Candidate_SubscribeToMailingListIsFalse_DoesNotCreateSubscription()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = false };

            request.Candidate.HasMailingListSubscription.Should().BeNull();
        }

        [Fact]
        public void Candidate_DoesNotProvidePostcode_StillCreatesEventsSubscription()
        {
            var request = new TeachingEventAddAttendee() { AddressPostcode = null };

            request.Candidate.HasEventsSubscription.Should().BeTrue();
        }

        [Fact]
        public void Candidate_AddressPostcode_IsFormatted()
        {
            var request = new TeachingEventAddAttendee() { AddressPostcode = "ky119yu" };

            request.Candidate.AddressPostcode.Should().Be("KY11 9YU");
        }

        [Fact]
        public void Candidate_WhenWalkIn_HasCorrectChannel()
        {
            var request = new TeachingEventAddAttendee() { IsWalkIn = true, EventId = Guid.NewGuid() };

            request.Candidate.TeachingEventRegistrations.First().ChannelId.Should().Be((int)TeachingEventRegistration.Channel.EventWalkIn);
        }

        [Fact]
        public void Candidate_WhenUnverifiedWalkIn_HasCorrectChannel()
        {
            var request = new TeachingEventAddAttendee() { IsWalkIn = true, IsVerified = false, EventId = Guid.NewGuid() };

            request.Candidate.TeachingEventRegistrations.First().ChannelId.Should().Be((int)TeachingEventRegistration.Channel.EventWalkInUnverified);
        }

        [Fact]
        public void ClearAttributesForUnverifiedAccess_ClearsPersonalAttributesExcludingMatchbackFields()
        {
            var request = new TeachingEventAddAttendee() { AddressTelephone = "1234567", AddressPostcode = "TE51NG" };

            request.ClearAttributesForUnverifiedAccess();

            request.AddressTelephone.Should().BeNull();
            request.AddressPostcode.Should().BeNull();
        }
    }
}
