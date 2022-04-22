using System;
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

        public CandidateUpserterTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _upserter = new CandidateUpserter(_mockCrm.Object, _mockJobClient.Object);
            _candidate = new Candidate() { Id = Guid.NewGuid(), Email = "test@test.com" };
        }

        [Fact]
        public void Upsert_WithCandidate_SavesCandidate()
        {
            _upserter.Upsert(_candidate);

            _mockCrm.Verify(mock => mock.Save(It.Is<Candidate>(c => IsMatch(_candidate, c))), Times.Once);
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

            _mockJobClient.Verify(x => x.Create(
                           It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<CandidateQualification>) && job.Method.Name == "Run" &&
                           IsMatch(qualification, (string)job.Args[0])),
                           It.IsAny<EnqueuedState>()));
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
        public void Upsert_WithTeachingEventRegistrations_SavesTeachingEventRegistrations()
        {
            var candidateId = Guid.NewGuid();
            var registration = new TeachingEventRegistration() { EventId = Guid.NewGuid() };
            _candidate.TeachingEventRegistrations.Add(registration);
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);

            _upserter.Upsert(_candidate);

            registration.CandidateId = candidateId;

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<TeachingEventRegistration>) && job.Method.Name == "Run" &&
               IsMatch(registration, (string)job.Args[0])),
               It.IsAny<EnqueuedState>()));
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
        public void Upsert_WithPrivacyPolicy_SavesPrivacyPolicy()
        {
            var candidateId = Guid.NewGuid();
            var policy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() };
            _candidate.PrivacyPolicy = policy;
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);

            _upserter.Upsert(_candidate);

            policy.CandidateId = candidateId;

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>) && job.Method.Name == "Run" &&
               IsMatch(policy, (string)job.Args[0])),
               It.IsAny<EnqueuedState>()));
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
