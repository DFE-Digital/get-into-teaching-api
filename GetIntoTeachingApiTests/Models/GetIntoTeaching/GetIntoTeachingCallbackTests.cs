using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching
{
    public class GetIntoTeachingCallbackTests
    {
        [Fact]
        public void Constructor_WithCandidate_MapsCorrectly()
        {
            var candidate = new Candidate()
            {
                Id = Guid.NewGuid(),
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                AddressTelephone = "00123456789",
            };

            var response = new GetIntoTeachingCallback(candidate);

            response.CandidateId.Should().Be(candidate.Id);
            response.Email.Should().Be(candidate.Email);
            response.FirstName.Should().Be(candidate.FirstName);
            response.LastName.Should().Be(candidate.LastName);
            response.AddressTelephone.Should().Be(candidate.AddressTelephone[2..]);
        }

        [Fact]
        public void Candidate_MapsCorrectly()
        {
            var request = new GetIntoTeachingCallback()
            {
                CandidateId = Guid.NewGuid(),
                AcceptedPolicyId = Guid.NewGuid(),
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                AddressTelephone = "123456789",
                PhoneCallScheduledAt = DateTime.UtcNow,
                TalkingPoints = "Talking points",
            };

            var candidate = request.Candidate;

            candidate.Id.Should().Be(request.CandidateId);
            candidate.Email.Should().Be(request.Email);
            candidate.FirstName.Should().Be(request.FirstName);
            candidate.LastName.Should().Be(request.LastName);
            candidate.AddressTelephone.Should().Be(request.AddressTelephone);

            candidate.PhoneCall.ScheduledAt.Should().Be((DateTime)request.PhoneCallScheduledAt);
            candidate.PhoneCall.Telephone.Should().Be(request.AddressTelephone);
            candidate.PhoneCall.ChannelId.Should().Be((int)PhoneCall.Channel.WebsiteCallbackRequest);
            candidate.PhoneCall.DestinationId.Should().Be((int)PhoneCall.Destination.Uk);
            candidate.PhoneCall.Subject.Should().Be("Scheduled phone call requested by John Doe");
            candidate.PhoneCall.TalkingPoints.Should().Be(request.TalkingPoints);

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be((Guid)request.AcceptedPolicyId);
            candidate.PrivacyPolicy.AcceptedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsGetIntoTeachingCallback()
        {
            var request = new GetIntoTeachingCallback() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.GetIntoTeachingCallback);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNotNull_IsNotChanged()
        {
            var request = new GetIntoTeachingCallback() { CandidateId = Guid.NewGuid() };

            request.Candidate.ChannelId.Should().BeNull();
            request.Candidate.ChangedPropertyNames.Should().NotContain("ChannelId");
        }
    }
}
