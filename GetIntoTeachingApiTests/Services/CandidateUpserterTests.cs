﻿using System;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Services;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using Xunit;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApiTests.Services
{
    public class CandidateUpserterTests
    {
        private readonly ICandidateUpserter _upserter;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Candidate _candidate;
        private readonly Candidate _existingCandidate;

        public CandidateUpserterTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _upserter = new CandidateUpserter(_mockCrm.Object, _mockJobClient.Object);
            _candidate = new Candidate() { Id = Guid.NewGuid(), Email = "test@test.com" };
            _existingCandidate = new Candidate() { Id = _candidate.Id, Email = "existing@email.com" };

            _mockCrm.Setup(m => m.GetCandidate((Guid)_candidate.Id)).Returns(_existingCandidate);
        }


        [Fact]
        public void Upsert_WithCandidate_SavesCandidate()
        {
            _upserter.Upsert(_candidate);

            _mockCrm.Verify(mock => mock.Save(It.Is<Candidate>(c => IsMatch(_candidate, c))), Times.Once);
        }

        [Fact]
        public void Upsert_WhenExistingCandidateFound_RetainsExistingCandidateEmail()
        {
            _upserter.Upsert(_candidate);

            _mockCrm.Verify(mock => mock.Save(It.Is<Candidate>(c => c.Email == _existingCandidate.Email)), Times.Once);
        }

        [Fact]
        public void Upsert_WhenExistingCandidateNotFound_SavesCandidate()
        {
            _mockCrm.Setup(m => m.GetCandidate((Guid)_candidate.Id)).Returns<Candidate>(null);

            _upserter.Upsert(_candidate);

            _mockCrm.Verify(mock => mock.Save(It.Is<Candidate>(c => IsMatch(_candidate, c))), Times.Once);
        }

        [Fact] 
        public void Upsert_WhenChangingEventSubscriptionFromSingleToLocal_RetainsLocalSubscription()
        {
            _mockCrm.Setup(m => m.CandidateAlreadyHasLocalEventSubscriptionType((Guid)_candidate.Id)).Returns(true);
            _candidate.EventsSubscriptionTypeId = (int)Candidate.SubscriptionType.SingleEvent;

            _upserter.Upsert(_candidate);

            _mockCrm.Verify(mock => mock.Save(It.Is<Candidate>(c => c.EventsSubscriptionTypeId ==
            (int)Candidate.SubscriptionType.LocalEvent)), Times.Once);
        }

        [Fact]
        public void Upsert_WithNullCandidateId_SetsIsNewRegistrantToTrue()
        {
            _candidate.Id = null;

            _upserter.Upsert(_candidate);

            _mockCrm.Verify(mock => mock.Save(It.Is<Candidate>(c => c.IsNewRegistrant)), Times.Once);
        }

        [Fact]
        public void Upsert_WithExistingCandidate_SetsIsNewRegistrantToFalse()
        {
            _upserter.Upsert(_candidate);

            _mockCrm.Verify(mock => mock.Save(It.Is<Candidate>(c => !c.IsNewRegistrant)), Times.Once);
        }

        [Fact]
        public void Upsert_WithQualifications_SavesQualifications()
        {
            var candidateId = Guid.NewGuid();
            var qualification = new CandidateQualification();
            _candidate.Qualifications.Add(qualification);
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);

            _upserter.Upsert(_candidate);

            qualification.CandidateId = candidateId;
            
            _mockCrm.Verify(mock => mock.Save(It.Is<CandidateQualification>(q => q.CandidateId == candidateId)), Times.Once);
        }

        [Fact]
        public void Upsert_WithPastTeachingPositions_SavesPastTeachingPositions()
        {
            var candidateId = Guid.NewGuid();
            var pastTeachingPosition = new CandidatePastTeachingPosition();
            _candidate.PastTeachingPositions.Add(pastTeachingPosition);
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);

            _upserter.Upsert(_candidate);

            pastTeachingPosition.CandidateId = candidateId;

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<CandidatePastTeachingPosition>) && job.Method.Name == "Run" &&
               IsMatch(pastTeachingPosition, (string)job.Args[0])),
               It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Upsert_WithApplicationForms_SavesApplicationForms()
        {
            var candidateId = Guid.NewGuid();
            var applicationForm = new ApplicationForm();
            _candidate.ApplicationForms.Add(applicationForm);
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);

            _upserter.Upsert(_candidate);

            applicationForm.CandidateId = candidateId;

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertApplicationFormJob) && job.Method.Name == "Run" &&
               IsMatch(applicationForm, (string)job.Args[0])),
               It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Upsert_WithSchoolExperiences_SavesSchoolExperiences()
        {
            var candidateId = Guid.NewGuid();
            var schoolExperience = new CandidateSchoolExperience();
            _candidate.SchoolExperiences.Add(schoolExperience);
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);

            _upserter.Upsert(_candidate);

            schoolExperience.CandidateId = candidateId;

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<CandidateSchoolExperience>) && job.Method.Name == "Run" &&
               IsMatch(schoolExperience, (string)job.Args[0])),
               It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Upsert_WhenNotAlreadyRegisteredForEvent_CreatesATeachingEventRegistrationEntity()
        {
            var registration = new TeachingEventRegistration() { EventId = Guid.NewGuid() };
            _candidate.TeachingEventRegistrations.Add(registration);
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = _candidate.Id);
            _mockCrm.Setup(m => m.CandidateYetToRegisterForTeachingEvent((Guid)_candidate.Id, registration.EventId)).Returns(true);

            _upserter.Upsert(_candidate);

            registration.CandidateId = (Guid)_candidate.Id;

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<TeachingEventRegistration>) && job.Method.Name == "Run" &&
               IsMatch(registration, (string)job.Args[0])),
               It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Upsert_WhenAlreadyRegisteredForEvent_DoesNotCreateTeachingEventRegistration()
        {
            var registration = new TeachingEventRegistration() { EventId = Guid.NewGuid() };
            _candidate.TeachingEventRegistrations.Add(registration);
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = _candidate.Id);
            _mockCrm.Setup(m => m.CandidateYetToRegisterForTeachingEvent((Guid)_candidate.Id, registration.EventId)).Returns(false);

            _upserter.Upsert(_candidate);

            registration.CandidateId = (Guid)_candidate.Id;

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<TeachingEventRegistration>)),
               It.IsAny<EnqueuedState>()), Times.Never);
        }

        [Fact]
        public void Upsert_WithPhoneCall_SavesPhoneCallAndIncrementsCallbackBookingQuotaNumberOfBookings()
        {
            var candidateId = Guid.NewGuid();
            var scheduledAt = DateTime.UtcNow.AddDays(3);
            var phoneCall = new PhoneCall() { ScheduledAt = scheduledAt };
            _candidate.PhoneCall = phoneCall;
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);

            _upserter.Upsert(_candidate);

            phoneCall.CandidateId = candidateId.ToString();

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<PhoneCall>) && job.Method.Name == "Run" &&
               IsMatch(phoneCall, (string)job.Args[0])),
               It.IsAny<EnqueuedState>()));

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(ClaimCallbackBookingSlotJob) && job.Method.Name == "Run" &&
               scheduledAt == (DateTime)job.Args[0]),
               It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Upsert_WhenPrivacyPolicyNotYetAccepted_SavesPrivacyPolicy()
        {
            var candidateId = Guid.NewGuid();
            var policy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() };
            _candidate.PrivacyPolicy = policy;
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);
            _mockCrm.Setup(m => m.CandidateYetToAcceptPrivacyPolicy(candidateId, policy.AcceptedPolicyId)).Returns(true);

            _upserter.Upsert(_candidate);

            policy.CandidateId = candidateId;

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>) && job.Method.Name == "Run" &&
               IsMatch(policy, (string)job.Args[0])),
               It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Upsert_WhenPrivacyPolicyAlreadyAccepted_DoesNotUpsertPrivacyPolicy()
        {
            var candidateId = Guid.NewGuid();
            var policy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() };
            _candidate.PrivacyPolicy = policy;
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);
            _mockCrm.Setup(m => m.CandidateYetToAcceptPrivacyPolicy(candidateId, policy.AcceptedPolicyId)).Returns(false);

            _upserter.Upsert(_candidate);

            policy.CandidateId = candidateId;

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>)),
               It.IsAny<EnqueuedState>()), Times.Never);
        }

        [Fact]
        public void Upsert_WhenPrivacyPolicyIsNull_DoesNotUpsertPrivacyPolicy()
        {
            _candidate.PrivacyPolicy = null;

            _upserter.Upsert(_candidate);

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>)),
               It.IsAny<EnqueuedState>()), Times.Never);
        }
        
        [Fact]
        public void Upsert_WithContactChannelCreation_SavesContactChannelCreations()
        {
            var candidateId = Guid.NewGuid();
            var contactChannelCreation = new ContactChannelCreation {
                CreationChannelActivityId = 222750000,
                CreationChannelServiceId = 222750001,
                CreationChannelSourceId = 222750002,
            };
            
            _candidate.ContactChannelCreations.Add(contactChannelCreation);

            _upserter.Upsert(_candidate);

            contactChannelCreation.CandidateId = candidateId;

            _mockJobClient.Verify(backgroundJobClient =>
                backgroundJobClient.Create(
                    It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<ContactChannelCreation>)),
                    It.IsAny<EnqueuedState>()), Times.Once);
        }

        private static bool IsMatch(object objectA, object objectB)
        {
            objectA.Should().BeEquivalentTo(objectB);
            return true;
        }

        private static bool IsMatch<T>(T modelA, string modelBJson)
        {
            var candidateB = modelBJson.DeserializeChangeTracked<T>();
            modelA.Should().BeEquivalentTo(candidateB);
            return true;
        }
    }
}
