using FluentAssertions;
using GetIntoTeachingApi.Models;
using System;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class MailingListAddMemberTests
    {
        [Fact]
        public void Candidate_MapsCorrectly()
        {
            var request = new MailingListAddMember()
            {
                CandidateId = Guid.NewGuid(),
                QualificationId = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                AcceptedPolicyId = Guid.NewGuid(),
                DescribeYourselfOptionId = 1,
                ConsiderationJourneyStageId = 2,
                UkDegreeGradeId = 3,
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
            candidate.EligibilityRulesPassed.Should().Be("false");
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
            candidate.Qualifications.First().Id.Should().Be(request.QualificationId);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsMailingList()
        {
            var request = new MailingListAddMember() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.MailingList);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNotNull_IsNull()
        {
            var request = new MailingListAddMember() { CandidateId = Guid.NewGuid() };

            request.Candidate.ChannelId.Should().BeNull();
        }

        [Fact]
        public void Candidate_SubscribeToEventsIsFalse_DoesNotCreateSubscription()
        {
            var request = new MailingListAddMember() { SubscribeToEvents = false };

            request.Candidate.Subscriptions.Count.Should().Be(1);
            request.Candidate.Subscriptions.Where(s => s.TypeId == (int)Subscription.ServiceType.Event).Count().Should().Be(0);
        }
    }
}
