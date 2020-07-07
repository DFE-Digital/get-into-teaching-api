using FluentAssertions;
using GetIntoTeachingApi.Models;
using System;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class MailingListAddMemberRequestTests
    {
        [Fact]
        public void Candidate_MapsCorrectly()
        {
            var mockPickListItem = new TypeEntity { Id = "123" };
            var mockEntityReference = new TypeEntity { Id = Guid.NewGuid().ToString() };
            var mockPrivacyPolicy = new PrivacyPolicy { Id = Guid.NewGuid() };

            var request = new MailingListAddMemberRequest()
            {
                CandidateId = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.Parse(mockEntityReference.Id),
                AcceptedPolicyId = (Guid)mockPrivacyPolicy.Id,
                DescribeYourselfOptionId = int.Parse(mockPickListItem.Id),
                ConsiderationJourneyStageId = int.Parse(mockPickListItem.Id),
                UkDegreeGradeId = int.Parse(mockPickListItem.Id),
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                Telephone = "1234567",
                AddressPostcode = "KY11 9YU",
                CallbackInformation = "Test information",
                SubscribeToEvents = true,
            };

            var candidate = request.Candidate;

            candidate.Id.Should().Equals(request.CandidateId);
            candidate.DescribeYourselfOptionId.Should().Be(request.DescribeYourselfOptionId);
            candidate.ConsiderationJourneyStageId.Should().Be(request.ConsiderationJourneyStageId);
            candidate.PreferredTeachingSubjectId.Should().Be(request.PreferredTeachingSubjectId);

            candidate.Email.Should().Be(request.Email);
            candidate.FirstName.Should().Be(request.FirstName);
            candidate.LastName.Should().Be(request.LastName);
            candidate.AddressPostcode.Should().Be(request.AddressPostcode);
            candidate.Telephone.Should().Be(request.Telephone);
            candidate.CallbackInformation.Should().Be(request.CallbackInformation);
            candidate.ChannelId.Should().BeNull();
            candidate.EligibilityRulesPassed.Should().Be("true");
            candidate.OptOutOfSms.Should().BeFalse();
            candidate.DoNotBulkEmail.Should().BeFalse();
            candidate.DoNotEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeTrue();
            candidate.DoNotPostalMail.Should().BeTrue();
            candidate.DoNotSendMm.Should().BeFalse();

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be(request.AcceptedPolicyId);
            candidate.Subscriptions.First().TypeId.Should().Be((int)Subscription.ServiceType.MailingList);
            candidate.Subscriptions.Last().TypeId.Should().Be((int)Subscription.ServiceType.Event);
            candidate.Qualifications.First().UkDegreeGradeId.Should().Be(request.UkDegreeGradeId);
        }

        [Theory]
        [InlineData("1234567", "true")]
        [InlineData(null, "false")]
        public void Candidate_EligibilityRulesPassed_MapsCorrectly(string telephone, string expected)
        {
            var request = new MailingListAddMemberRequest() { Telephone = telephone };

            request.Candidate.EligibilityRulesPassed.Should().Be(expected);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsMailingList()
        {
            var request = new MailingListAddMemberRequest() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.MailingList);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNotNull_IsNull()
        {
            var request = new MailingListAddMemberRequest() { CandidateId = Guid.NewGuid() };

            request.Candidate.ChannelId.Should().BeNull();
        }

        [Fact]
        public void Candidate_SubscribeToEventsIsFalse_DoesNotCreateSubscription()
        {
            var request = new MailingListAddMemberRequest() { SubscribeToEvents = false };

            request.Candidate.Subscriptions.Count.Should().Be(1);
            request.Candidate.Subscriptions.Where(s => s.TypeId == (int)Subscription.ServiceType.Event).Count().Should().Be(0);
        }
    }
}
