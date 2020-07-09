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
            var subscriptions = new List<Subscription>() { new Subscription() { TypeId = (int)Subscription.ServiceType.Event } };

            var candidate = new Candidate()
            {
                Id = Guid.NewGuid(),
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                Telephone = "1234567",
                AddressPostcode = "KY11 9YU",
                Subscriptions = subscriptions,
            };

            var response = new TeachingEventAddAttendee(candidate);

            response.CandidateId.Should().Be(candidate.Id);
            response.Email.Should().Be(candidate.Email);
            response.FirstName.Should().Be(candidate.FirstName);
            response.LastName.Should().Be(candidate.LastName);
            response.Telephone.Should().Be(candidate.Telephone);
            response.AddressPostcode.Should().Be(candidate.AddressPostcode);

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
                AcceptedPolicyId = Guid.NewGuid(),
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

            candidate.Email.Should().Be(request.Email);
            candidate.FirstName.Should().Be(request.FirstName);
            candidate.LastName.Should().Be(request.LastName);
            candidate.AddressPostcode.Should().Be(request.AddressPostcode);
            candidate.Telephone.Should().Be(request.Telephone);
            candidate.ChannelId.Should().BeNull();
            candidate.EligibilityRulesPassed.Should().Be("false");
            candidate.OptOutOfSms.Should().BeFalse();
            candidate.DoNotBulkEmail.Should().BeTrue();
            candidate.DoNotEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeTrue();
            candidate.DoNotPostalMail.Should().BeTrue();
            candidate.DoNotSendMm.Should().BeTrue();

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be((Guid)request.AcceptedPolicyId);
            candidate.Subscriptions.First().TypeId.Should().Be((int)Subscription.ServiceType.Event);
            candidate.Subscriptions.Last().TypeId.Should().Be((int)Subscription.ServiceType.MailingList);
            candidate.TeachingEventRegistrations.First().EventId.Should().Equals(request.EventId);
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

            request.Candidate.Subscriptions.Count.Should().Be(0);
        }

        [Fact]
        public void Candidate_SubscribeToEventsIsFalse_DoesNotCreateSubscription()
        {
            var request = new TeachingEventAddAttendee() { SubscribeToEvents = false };

            request.Candidate.Subscriptions.Count.Should().Be(0);
        }
    }
}
