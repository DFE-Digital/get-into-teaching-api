using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using System;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching
{
    public class GetIntoTeachingCallbackTests
    {
        [Fact]
        public void Constructor_WithExistingCandidate_MapsCorrectly()
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
        public void ExistingCandidate_MapsCorrectly()
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
                CreationChannelSourceId = 222750003,
                CreationChannelServiceId = 222750002,
                CreationChannelActivityId = 222750001,
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
            var request = new GetIntoTeachingCallback()
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
