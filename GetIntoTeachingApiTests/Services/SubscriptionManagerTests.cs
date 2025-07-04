﻿using System;
using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class SubscriptionManagerTests
    {
        [Fact]
        public void SubscribeToMailingList_DefaultChannel_CorrectSubscription()
        {
            var candidate = new Candidate();

            SubscriptionManager.SubscribeToMailingList(candidate, DateTime.UtcNow);

            candidate.HasMailingListSubscription.Should().BeTrue();
            candidate.MailingListSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Subscribed);
            candidate.MailingListSubscriptionStartAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            candidate.MailingListSubscriptionDoNotBulkEmail.Should().BeFalse();
            candidate.MailingListSubscriptionDoNotBulkPostalMail.Should().BeTrue();
            candidate.MailingListSubscriptionDoNotPostalMail.Should().BeTrue();
            candidate.MailingListSubscriptionDoNotSendMm.Should().BeFalse();
            candidate.MailingListSubscriptionDoNotEmail.Should().BeFalse();
        }

        [Fact]
        public void SubscribeToMailingList_CustomChannel_CorrectSubscription()
        {
            var candidate = new Candidate();

            SubscriptionManager.SubscribeToMailingList(candidate, DateTime.UtcNow);

            candidate.MailingListSubscriptionChannelId.Should().Be((int) Candidate.SubscriptionChannel.Subscribed);
        }

        [Fact]
        public void SubscribeToMailingList_NewCandidate_CorrectConsent()
        {
            var candidate = new Candidate();

            SubscriptionManager.SubscribeToMailingList(candidate, DateTime.UtcNow);

            candidate.OptOutOfSms.Should().BeFalse();
            candidate.DoNotBulkEmail.Should().BeFalse();
            candidate.DoNotEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeTrue();
            candidate.DoNotPostalMail.Should().BeTrue();
            candidate.DoNotSendMm.Should().BeFalse();
        }

        [Fact]
        public void SubscribeToMailingList_ExistingCandidate_DoesNotOptOutIfAlreadyConsented()
        {
            var candidate = new Candidate() { DoNotBulkPostalMail = false, DoNotPostalMail = false };

            SubscriptionManager.SubscribeToMailingList(candidate, DateTime.UtcNow);

            candidate.DoNotBulkPostalMail.Should().BeFalse();
            candidate.DoNotPostalMail.Should().BeFalse();
        }

        [Fact]
        public void SubscribeToEvents_CandidateHasAddressPostcodeAndDefaultChannel_CorrectSubscription()
        {
            var candidate = new Candidate() { AddressPostcode = "TE5 7IN" };

            SubscriptionManager.SubscribeToEvents(candidate, DateTime.UtcNow);

            candidate.HasEventsSubscription.Should().BeTrue();
            candidate.EventsSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Subscribed);
            candidate.EventsSubscriptionStartAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            candidate.EventsSubscriptionDoNotBulkEmail.Should().BeFalse();
            candidate.EventsSubscriptionDoNotBulkPostalMail.Should().BeTrue();
            candidate.EventsSubscriptionDoNotPostalMail.Should().BeTrue();
            candidate.EventsSubscriptionDoNotSendMm.Should().BeFalse();
            candidate.EventsSubscriptionDoNotEmail.Should().BeFalse();
            candidate.EventsSubscriptionTypeId.Should().Be((int)Candidate.SubscriptionType.LocalEvent);
        }

        [Fact]
        public void SubscribeToEvents_CandidateHasNoAddressPostcodeAndCustomChannel_CorrectSubscription()
        {
            var candidate = new Candidate();

            SubscriptionManager.SubscribeToEvents(candidate, DateTime.UtcNow);

            candidate.EventsSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Subscribed);
            candidate.EventsSubscriptionTypeId.Should().Be((int)Candidate.SubscriptionType.SingleEvent);
        }

        [Fact]
        public void SubscribeToEvents_NewCandidate_CorrectConsent()
        {
            var candidate = new Candidate();

            SubscriptionManager.SubscribeToEvents(candidate, DateTime.UtcNow);

            candidate.OptOutOfSms.Should().BeFalse();
            candidate.DoNotBulkEmail.Should().BeFalse();
            candidate.DoNotEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeTrue();
            candidate.DoNotPostalMail.Should().BeTrue();
            candidate.DoNotSendMm.Should().BeTrue();
        }

        [Fact]
        public void SubscribeToEvents_ExistingCandidate_DoesNotOptOutIfAlreadyConsented()
        {
            var candidate = new Candidate() { DoNotBulkPostalMail = false, DoNotPostalMail = false };

            SubscriptionManager.SubscribeToEvents(candidate, DateTime.UtcNow);

            candidate.DoNotBulkPostalMail.Should().BeFalse();
            candidate.DoNotPostalMail.Should().BeFalse();
        }


        [Fact]
        public void SubscribeToTeacherTrainingAdviser_CustomChannel_UsesChannel()
        {
            var candidate = new Candidate();

            SubscriptionManager.SubscribeToTeacherTrainingAdviser(candidate, DateTime.UtcNow);

            candidate.TeacherTrainingAdviserSubscriptionChannelId.Should().Be((int) Candidate.SubscriptionChannel.Subscribed);
        }

        [Fact]
        public void SubscribeToTeacherTrainingAdviser_NotReturningToTeaching_CorrectSubscription()
        {
            var candidate = new Candidate()
            {
                TypeId = (int)Candidate.Type.InterestedInTeacherTraining,
            };

            SubscriptionManager.SubscribeToTeacherTrainingAdviser(candidate, DateTime.UtcNow);

            candidate.HasTeacherTrainingAdviserSubscription.Should().BeTrue();
            candidate.TeacherTrainingAdviserSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Subscribed);
            candidate.TeacherTrainingAdviserSubscriptionStartAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            candidate.TeacherTrainingAdviserSubscriptionDoNotBulkEmail.Should().BeFalse();
            candidate.TeacherTrainingAdviserSubscriptionDoNotBulkPostalMail.Should().BeTrue();
            candidate.TeacherTrainingAdviserSubscriptionDoNotPostalMail.Should().BeTrue();
            candidate.TeacherTrainingAdviserSubscriptionDoNotSendMm.Should().BeFalse();
            candidate.TeacherTrainingAdviserSubscriptionDoNotEmail.Should().BeFalse();
        }

        [Fact]
        public void SubscribeToTeacherTrainingAdviser_ReturningToTeaching_CorrectSubscription()
        {
            var candidate = new Candidate()
            {
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
            };

            SubscriptionManager.SubscribeToTeacherTrainingAdviser(candidate, DateTime.UtcNow);

            candidate.HasTeacherTrainingAdviserSubscription.Should().BeTrue();
            candidate.TeacherTrainingAdviserSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Subscribed);
            candidate.TeacherTrainingAdviserSubscriptionStartAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            candidate.TeacherTrainingAdviserSubscriptionDoNotBulkEmail.Should().BeTrue();
            candidate.TeacherTrainingAdviserSubscriptionDoNotBulkPostalMail.Should().BeTrue();
            candidate.TeacherTrainingAdviserSubscriptionDoNotPostalMail.Should().BeTrue();
            candidate.TeacherTrainingAdviserSubscriptionDoNotSendMm.Should().BeTrue();
            candidate.TeacherTrainingAdviserSubscriptionDoNotEmail.Should().BeFalse();
        }

        [Fact]
        public void SubscribeToTeacherTrainingAdviser_NewNonReturnerCandidate_CorrectConsent()
        {
            var candidate = new Candidate();

            SubscriptionManager.SubscribeToTeacherTrainingAdviser(candidate, DateTime.UtcNow);

            candidate.OptOutOfSms.Should().BeFalse();
            candidate.DoNotBulkEmail.Should().BeFalse();
            candidate.DoNotEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeTrue();
            candidate.DoNotPostalMail.Should().BeTrue();
            candidate.DoNotSendMm.Should().BeFalse();
        }

        [Fact]
        public void SubscribeToTeacherTrainingAdviser_NewReturnerCandidate_CorrectConsent()
        {
            var candidate = new Candidate() { TypeId = (int)Candidate.Type.ReturningToTeacherTraining };

            SubscriptionManager.SubscribeToTeacherTrainingAdviser(candidate, DateTime.UtcNow);

            candidate.OptOutOfSms.Should().BeFalse();
            candidate.DoNotBulkEmail.Should().BeTrue();
            candidate.DoNotEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeTrue();
            candidate.DoNotPostalMail.Should().BeTrue();
            candidate.DoNotSendMm.Should().BeTrue();
        }

        [Fact]
        public void SubscribeToTeacherTrainingAdviser_ExistingReturnerCandidate_DoesNotOptOutIfAlreadyConsented()
        {
            var candidate = new Candidate()
            {
                DoNotBulkEmail = false,
                DoNotBulkPostalMail = false,
                DoNotPostalMail = false,
                DoNotSendMm = false,
                TypeId = (int)Candidate.Type.ReturningToTeacherTraining,
            };

            SubscriptionManager.SubscribeToTeacherTrainingAdviser(candidate, DateTime.UtcNow);

            candidate.DoNotBulkEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeFalse();
            candidate.DoNotPostalMail.Should().BeFalse();
            candidate.DoNotSendMm.Should().BeFalse();
        }
    }
}
