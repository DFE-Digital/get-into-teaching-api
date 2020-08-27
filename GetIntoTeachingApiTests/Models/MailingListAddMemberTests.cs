using FluentAssertions;
using GetIntoTeachingApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class MailingListAddMemberTests
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

            var response = new MailingListAddMember(candidate);

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
            var request = new MailingListAddMember()
            {
                CandidateId = Guid.NewGuid(),
                QualificationId = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                AcceptedPolicyId = Guid.NewGuid(),
                ConsiderationJourneyStageId = 1,
                DegreeStatusId = 2,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                Telephone = "1234567",
                AddressPostcode = "KY11 9YU",
                SubscribeToEvents = true,
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
            candidate.DoNotBulkPostalMail.Should().BeTrue();
            candidate.DoNotPostalMail.Should().BeTrue();
            candidate.DoNotSendMm.Should().BeFalse();
            candidate.EligibilityRulesPassed.Should().Be("false");

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be((Guid)request.AcceptedPolicyId);

            candidate.HasMailingListSubscription.Should().BeTrue();
            candidate.MailingListSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.MailingList);
            candidate.MailingListSubscriptionStartAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            candidate.MailingListSubscriptionDoNotBulkEmail.Should().BeFalse();
            candidate.MailingListSubscriptionDoNotBulkPostalMail.Should().BeTrue();
            candidate.MailingListSubscriptionDoNotPostalMail.Should().BeTrue();
            candidate.MailingListSubscriptionDoNotSendMm.Should().BeFalse();
            candidate.MailingListSubscriptionDoNotEmail.Should().BeFalse();

            candidate.HasEventsSubscription.Should().BeTrue();
            candidate.EventsSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Events);
            candidate.EventsSubscriptionStartAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            candidate.EventsSubscriptionDoNotBulkEmail.Should().BeFalse();
            candidate.EventsSubscriptionDoNotBulkPostalMail.Should().BeTrue();
            candidate.EventsSubscriptionDoNotPostalMail.Should().BeTrue();
            candidate.EventsSubscriptionDoNotSendMm.Should().BeFalse();
            candidate.EventsSubscriptionDoNotEmail.Should().BeFalse();

            candidate.Qualifications.First().DegreeStatusId.Should().Be(request.DegreeStatusId);
            candidate.Qualifications.First().TypeId.Should().Be((int)CandidateQualification.DegreeType.Degree);
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

            request.Candidate.HasEventsSubscription.Should().BeNull();
        }
    }
}
