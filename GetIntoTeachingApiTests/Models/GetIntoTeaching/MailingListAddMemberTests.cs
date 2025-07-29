using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching
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
                AddressPostcode = "KY11 9YU",
                WelcomeGuideVariant = "variant1",
                Qualifications = qualifications,
                HasEventsSubscription = true,
                HasTeacherTrainingAdviserSubscription = true,
            };

            var response = new MailingListAddMember(candidate);

            response.CandidateId.Should().Be(candidate.Id);
            response.PreferredTeachingSubjectId.Should().Be(candidate.PreferredTeachingSubjectId);
            response.ConsiderationJourneyStageId.Should().Be(candidate.ConsiderationJourneyStageId);
            response.Email.Should().Be(candidate.Email);
            response.FirstName.Should().Be(candidate.FirstName);
            response.LastName.Should().Be(candidate.LastName);
            response.AddressPostcode.Should().Be(candidate.AddressPostcode);
            response.WelcomeGuideVariant.Should().Be(candidate.WelcomeGuideVariant);

            response.QualificationId.Should().Be(latestQualification.Id);
            response.DegreeStatusId.Should().Be(latestQualification.DegreeStatusId);

            response.AlreadySubscribedToEvents.Should().BeTrue();
            response.AlreadySubscribedToMailingList.Should().BeFalse();
            response.AlreadySubscribedToTeacherTrainingAdviser.Should().BeTrue();
        }

        [Fact]
        public void ExistingCandidate_MapsCorrectly()
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
                Situation = 123456,
                AddressPostcode = "KY11 9YU",
                WelcomeGuideVariant = "variant1",
                Citizenship = 997755,
                VisaStatus = 119800,
                Location = 234353,
                CreationChannelSourceId = 222750003,
                CreationChannelServiceId = 222750002,
                CreationChannelActivityId = 222750001
            };

            var candidate = request.Candidate;

            candidate.Id.Should().Be(request.CandidateId);
            candidate.ConsiderationJourneyStageId.Should().Be(request.ConsiderationJourneyStageId);
            candidate.PreferredTeachingSubjectId.Should().Be(request.PreferredTeachingSubjectId);

            candidate.Email.Should().Be(request.Email);
            candidate.FirstName.Should().Be(request.FirstName);
            candidate.LastName.Should().Be(request.LastName);
            candidate.Situation.Should().Be(request.Situation);
            candidate.AddressPostcode.Should().Be(request.AddressPostcode);
            candidate.ChannelId.Should().BeNull();
            candidate.OptOutOfSms.Should().BeFalse();
            candidate.DoNotBulkEmail.Should().BeFalse();
            candidate.DoNotEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeTrue();
            candidate.DoNotPostalMail.Should().BeTrue();
            candidate.DoNotSendMm.Should().BeFalse();
            candidate.EligibilityRulesPassed.Should().Be("false");
            candidate.PreferredPhoneNumberTypeId.Should().Be((int)Candidate.PhoneNumberType.Home);
            candidate.PreferredContactMethodId.Should().Be((int)Candidate.ContactMethod.Any);
            candidate.GdprConsentId.Should().Be((int)Candidate.GdprConsent.Consent);
            candidate.OptOutOfGdpr.Should().BeFalse();
            candidate.WelcomeGuideVariant.Should().Be(request.WelcomeGuideVariant);
            candidate.Citizenship.Should().Be(request.Citizenship);
            candidate.VisaStatus.Should().Be(request.VisaStatus);
            candidate.Location.Should().Be(request.Location);

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be((Guid)request.AcceptedPolicyId);
            candidate.PrivacyPolicy.AcceptedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));

            candidate.HasMailingListSubscription.Should().BeTrue();
            candidate.HasEventsSubscription.Should().BeTrue();

            candidate.Qualifications.First().DegreeStatusId.Should().Be(request.DegreeStatusId);
            candidate.Qualifications.First().TypeId.Should().Be((int)CandidateQualification.DegreeType.Degree);
            candidate.Qualifications.First().Id.Should().Be(request.QualificationId);
            
            var contactChannelCreation = candidate.ContactChannelCreations.First();
            contactChannelCreation.CreationChannel.Should().Be(false);
            contactChannelCreation.CreationChannelSourceId.Should().Be(request.CreationChannelSourceId);
            contactChannelCreation.CreationChannelServiceId.Should().Be(request.CreationChannelServiceId);
            contactChannelCreation.CreationChannelActivityId.Should().Be(request.CreationChannelActivityId);
            candidate.ChannelId.Should().Be(null);
        }
        
        [Fact]
        public void NewCandidate_MapsCreationChannelCorrectly()
        {
            var request = new MailingListAddMember()
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
        public void Candidate_AddressPostcodeNotProvided_EventsSubscriptionIsNotCreated()
        {
            var request = new MailingListAddMember()
            {
                AddressPostcode = null,
            };

            request.Candidate.HasEventsSubscription.Should().BeNull();
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsMailingList_WithoutDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "1");
            
            var request = new MailingListAddMember() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.MailingList);
            
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
        }
        
        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsMailingList_WithDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "0");
            
            var request = new MailingListAddMember() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be(null);
            
            var ccc = request.Candidate.ContactChannelCreations.First();
            ccc.CreationChannel.Should().Be(true);
            ccc.CreationChannelSourceId.Should().Be((int?)ContactChannelCreation.CreationChannelSource.GITWebsite);
            ccc.CreationChannelServiceId.Should().Be((int?)ContactChannelCreation.CreationChannelService.MailingList);
            ccc.CreationChannelActivityId.Should().Be(null);
            
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNotNull_IsNotChanged()
        {
            var request = new MailingListAddMember() { CandidateId = Guid.NewGuid() };

            request.Candidate.ChannelId.Should().BeNull();
            request.Candidate.ChangedPropertyNames.Should().NotContain("ChannelId");
        }

        [Fact]
        public void Candidate_WhenChannelIsProvided_SetsOnAllModels_WithoutDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "1");
            
            var request = new MailingListAddMember() { ChannelId = 123, AddressPostcode = "TE7 8KE" };

            request.Candidate.ChannelId.Should().Be(123);
            request.Candidate.MailingListSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Subscribed);
            request.Candidate.EventsSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Subscribed);
            
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
        }
        
        [Fact]
        public void Candidate_WhenChannelIsProvided_SetsOnAllModels_WithDefaultCreationChannels()
        {
            var previous = Environment.GetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS");
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", "0");
            
            var request = new MailingListAddMember() { ChannelId = 123, AddressPostcode = "TE7 8KE" };
            
            request.Candidate.ChannelId.Should().Be(null);
            
            var ccc = request.Candidate.ContactChannelCreations.First();
            ccc.CreationChannel.Should().Be(true);
            ccc.CreationChannelSourceId.Should().Be((int?)ContactChannelCreation.CreationChannelSource.GITWebsite);
            ccc.CreationChannelServiceId.Should().Be((int?)ContactChannelCreation.CreationChannelService.MailingList);
            ccc.CreationChannelActivityId.Should().Be(null);
            
            request.Candidate.MailingListSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Subscribed);
            request.Candidate.EventsSubscriptionChannelId.Should().Be((int)Candidate.SubscriptionChannel.Subscribed);
            
            Environment.SetEnvironmentVariable("DISABLE_DEFAULT_CREATION_CHANNELS", previous);
        }

        [Fact]
        public void Candidate_AddressPostcode_IsFormatted()
        {
            var request = new MailingListAddMember() { AddressPostcode = "ky119yu" };

            request.Candidate.AddressPostcode.Should().Be("KY11 9YU");
        }
    }
}
