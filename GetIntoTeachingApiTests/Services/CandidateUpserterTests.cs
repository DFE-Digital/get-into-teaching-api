using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class CandidateUpserterTests
    {
        private readonly ICandidateUpserter _upserter;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Candidate _candidate;

        public CandidateUpserterTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _upserter = new CandidateUpserter(_mockCrm.Object);
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
            _mockCrm.Verify(mock => mock.Save(It.Is<CandidateQualification>(q => IsMatch(qualification, q))), Times.Once);
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
            _mockCrm.Verify(mock => mock.Save(It.Is<CandidatePastTeachingPosition>(p => IsMatch(pastTeachingPosition, p))), Times.Once);
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
            _mockCrm.Verify(mock => mock.Save(It.Is<TeachingEventRegistration>(r => IsMatch(registration, r))), Times.Once);
        }

        [Fact]
        public void Upsert_WithPhoneCall_SavesPhoneCallAndIncrementsCallbackBookingQuotaNumberOfBookings()
        {
            var candidateId = Guid.NewGuid();
            var scheduledAt = DateTime.UtcNow.AddDays(3);
            var phoneCall = new PhoneCall() { ScheduledAt = scheduledAt };
            _candidate.PhoneCall = phoneCall;
            var quota = new CallbackBookingQuota() { StartAt = scheduledAt, NumberOfBookings = 5, Quota = 10 };
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);
            _mockCrm.Setup(mock => mock.GetCallbackBookingQuota(scheduledAt)).Returns(quota);

            _upserter.Upsert(_candidate);

            phoneCall.CandidateId = candidateId.ToString();
            _mockCrm.Verify(mock => mock.Save(It.Is<PhoneCall>(p => IsMatch(phoneCall, p))), Times.Once);
            _mockCrm.Verify(mock => mock.Save(It.Is<CallbackBookingQuota>(q => IsMatch(quota, q))), Times.Once);
            quota.NumberOfBookings.Should().Be(6);
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
            _mockCrm.Verify(mock => mock.Save(It.Is<CandidatePrivacyPolicy>(p => IsMatch(policy, p))), Times.Once);
        }

        [Fact]
        public void Upsert_WithPhoneCallButMatchingQuotaIsAlreadyFull_DoesNotIncrementsCallbackBookingQuotaNumberOfBookings()
        {
            var candidateId = Guid.NewGuid();
            var scheduledAt = DateTime.UtcNow.AddDays(3);
            var phoneCall = new PhoneCall() { ScheduledAt = scheduledAt };
            var quota = new CallbackBookingQuota() { StartAt = scheduledAt, NumberOfBookings = 5, Quota = 5 };
            _candidate.PhoneCall = phoneCall;
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);
            _mockCrm.Setup(mock => mock.GetCallbackBookingQuota(scheduledAt)).Returns(quota);

            _upserter.Upsert(_candidate);

            _mockCrm.Verify(mock => mock.Save(It.IsAny<CallbackBookingQuota>()), Times.Never);
            quota.NumberOfBookings.Should().Be(5);
        }

        private bool IsMatch(object objectA, object objectB)
        {
            objectA.Should().BeEquivalentTo(objectB);
            return true;
        }
    }
}
