using FluentAssertions;
using GetIntoTeachingApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingEventAddAttendeeTests
    {
        [Fact]
        public void Constructor_WithCandidate_MapsCorrectly()
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
        }

        [Fact]
        public void Candidate_MapsCorrectly()
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
            };

            var candidate = request.Candidate;

            candidate.Id.Should().Equals(request.CandidateId);
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
            candidate.PrivacyPolicy.AcceptedAt.Should().BeCloseTo(DateTime.UtcNow);

            candidate.HasEventsSubscription.Should().BeTrue();
            candidate.HasMailingListSubscription.Should().BeTrue();

            candidate.TeachingEventRegistrations.First().EventId.Should().Equals(request.EventId);
            candidate.TeachingEventRegistrations.First().ChannelId.Should().Be((int)TeachingEventRegistration.Channel.Event);
            candidate.TeachingEventRegistrations.First().IsCancelled.Should().BeFalse();
            candidate.TeachingEventRegistrations.First().RegistrationNotificationSeen.Should().BeFalse();

            candidate.Qualifications.First().DegreeStatusId.Should().Be(request.DegreeStatusId);
            candidate.Qualifications.First().TypeId.Should().Be((int)CandidateQualification.DegreeType.Degree);
            candidate.Qualifications.First().Id.Should().Be(request.QualificationId);
        }

        [Fact]
        public void Candidate_SubscribeToMailingListIsFalse_ConsentIsCorrect()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = false, AddressPostcode = null };

            request.Candidate.DoNotSendMm.Should().BeTrue();
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsMailingList()
        {
            var request = new TeachingEventAddAttendee() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.Event);
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
    }
}
