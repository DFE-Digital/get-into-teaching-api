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
                Telephone = "1234567",
                AddressPostcode = "KY11 9YU",
                Qualifications = qualifications,
                HasEventsSubscription = true,
            };

            var response = new TeachingEventAddAttendee(candidate);

            response.CandidateId.Should().Be(candidate.Id);
            response.PreferredTeachingSubjectId.Should().Be(candidate.PreferredTeachingSubjectId);
            response.ConsiderationJourneyStageId.Should().Be(candidate.ConsiderationJourneyStageId);
            response.Email.Should().Be(candidate.Email);
            response.FirstName.Should().Be(candidate.FirstName);
            response.LastName.Should().Be(candidate.LastName);
            response.Telephone.Should().Be(candidate.Telephone);
            response.AddressPostcode.Should().Be(candidate.AddressPostcode);

            response.QualificationId.Should().Be(latestQualification.Id);
            response.DegreeStatusId.Should().Be(latestQualification.DegreeStatusId);

            response.AlreadySubscribedToEvents.Should().BeTrue();
            response.AlreadySubscribedToMailingList.Should().BeFalse();
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
                ConsiderationJourneyStageId = 1,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                Telephone = "1234567",
                AddressPostcode = "KY11 9YU",
                SubscribeToEvents = true,
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
            candidate.Telephone.Should().Be(request.Telephone);
            candidate.ChannelId.Should().BeNull();
            candidate.OptOutOfSms.Should().BeFalse();
            candidate.DoNotBulkEmail.Should().BeFalse();
            candidate.DoNotEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeFalse();
            candidate.DoNotPostalMail.Should().BeFalse();
            candidate.DoNotSendMm.Should().BeFalse();

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be((Guid)request.AcceptedPolicyId);

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
        public void Candidate_SubscribeToMailingListIsTrue_CorrectOptInStatus()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = true, SubscribeToEvents = false };

            var candidate = request.Candidate;

            candidate.HasMailingListSubscription.Should().BeTrue();
            candidate.MailingListSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.MailingList);
            candidate.MailingListSubscriptionStartAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            candidate.MailingListSubscriptionDoNotBulkEmail.Should().BeFalse();
            candidate.MailingListSubscriptionDoNotBulkPostalMail.Should().BeTrue();
            candidate.MailingListSubscriptionDoNotPostalMail.Should().BeTrue();
            candidate.MailingListSubscriptionDoNotSendMm.Should().BeFalse();
            candidate.MailingListSubscriptionDoNotEmail.Should().BeFalse();
        }

        [Fact]
        public void Candidate_SubscribeToEventsIsTrue_CorrectOptInStatus()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToEvents = true, SubscribeToMailingList = false };

            var candidate = request.Candidate;

            candidate.HasEventsSubscription.Should().BeTrue();
            candidate.EventsSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Events);
            candidate.EventsSubscriptionStartAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            candidate.EventsSubscriptionDoNotBulkEmail.Should().BeFalse();
            candidate.EventsSubscriptionDoNotBulkPostalMail.Should().BeTrue();
            candidate.EventsSubscriptionDoNotPostalMail.Should().BeTrue();
            candidate.EventsSubscriptionDoNotSendMm.Should().BeFalse();
            candidate.EventsSubscriptionDoNotEmail.Should().BeFalse();
        }

        [Fact]
        public void Candidate_SubscribeToEventsIsFalse_CorrectOptInStatus()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToEvents = false, SubscribeToMailingList = false };

            var candidate = request.Candidate;

            candidate.HasEventsSubscription.Should().BeTrue();
            candidate.EventsSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Events);
            candidate.EventsSubscriptionStartAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            candidate.EventsSubscriptionDoNotBulkEmail.Should().BeTrue();
            candidate.EventsSubscriptionDoNotBulkPostalMail.Should().BeTrue();
            candidate.EventsSubscriptionDoNotPostalMail.Should().BeTrue();
            candidate.EventsSubscriptionDoNotSendMm.Should().BeTrue();
            candidate.EventsSubscriptionDoNotEmail.Should().BeFalse();
        }

        [Fact]
        public void Candidate_SubscribeToMailingListIsFalse_ConsentIsCorrect()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = false, SubscribeToEvents = true };

            request.Candidate.DoNotBulkPostalMail.Should().BeTrue();
            request.Candidate.DoNotPostalMail.Should().BeTrue();
        }

        [Fact]
        public void Candidate_SubscribeToMailingListAndEventsAreFalse_ConsentIsCorrect()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = false, SubscribeToEvents = false };

            request.Candidate.DoNotBulkEmail.Should().BeTrue();
            request.Candidate.DoNotSendMm.Should().BeTrue();
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsMailingList()
        {
            var request = new TeachingEventAddAttendee() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.Event);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNotNull_IsNull()
        {
            var request = new TeachingEventAddAttendee() { CandidateId = Guid.NewGuid() };

            request.Candidate.ChannelId.Should().BeNull();
        }

        [Fact]
        public void Candidate_SubscribeToMailingListIsFalse_DoesNotCreateSubscription()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToMailingList = false };

            request.Candidate.HasMailingListSubscription.Should().BeNull();
        }

        [Fact]
        public void Candidate_SubscribeToEventsIsFalse_StillCreatesEventsSubscription()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToEvents = false };

            request.Candidate.HasEventsSubscription.Should().BeTrue();
        }
    }
}
