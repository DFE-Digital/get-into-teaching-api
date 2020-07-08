using FluentAssertions;
using GetIntoTeachingApi.Models;
using System;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingEventAddAttendeeRequestTests
    {
        [Fact]
        public void Candidate_MapsCorrectly()
        {
            var request = new TeachingEventAddAttendeeRequest()
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

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be(request.AcceptedPolicyId);
            candidate.Subscriptions.First().TypeId.Should().Be((int)Subscription.ServiceType.Event);
            candidate.Subscriptions.Last().TypeId.Should().Be((int)Subscription.ServiceType.MailingList);
            candidate.TeachingEventRegistrations.First().EventId.Should().Equals(request.EventId);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsMailingList()
        {
            var request = new TeachingEventAddAttendeeRequest() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.Event);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNotNull_IsNull()
        {
            var request = new TeachingEventAddAttendeeRequest() { CandidateId = Guid.NewGuid() };

            request.Candidate.ChannelId.Should().BeNull();
        }

        [Fact]
        public void Candidate_SubscribeToMailingListIsFalse_DoesNotCreateSubscription()
        {
            var request = new TeachingEventAddAttendeeRequest() { SubscribeToMailingList = false };

            request.Candidate.Subscriptions.Count.Should().Be(0);
        }

        [Fact]
        public void Candidate_SubscribeToEventsIsFalse_DoesNotCreateSubscription()
        {
            var request = new TeachingEventAddAttendeeRequest() { SubscribeToEvents = false };

            request.Candidate.Subscriptions.Count.Should().Be(0);
        }
    }
}
