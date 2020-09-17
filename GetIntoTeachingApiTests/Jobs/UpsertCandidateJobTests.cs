using System;
using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class UpsertCandidateJobTests
    {
        private readonly Mock<IPerformContextAdapter> _mockContext;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Candidate _candidate;
        private readonly UpsertCandidateJob _job;
        private readonly Mock<ILogger<UpsertCandidateJob>> _mockLogger;

        public UpsertCandidateJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockLogger = new Mock<ILogger<UpsertCandidateJob>>();
            _candidate = new Candidate() { Email = "test@test.com" };
            _job = new UpsertCandidateJob(new Env(), _mockCrm.Object, _mockNotifyService.Object,
                _mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public void Run_OnSuccess_SavesCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(_candidate), Times.Once);
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Succeeded");
        }

        [Fact]
        public void Run_WithTeachingEventRegistrationsOnSuccess_SavesTeachingEventRegistrations()
        {
            var candidateId = Guid.NewGuid();
            var registration = new TeachingEventRegistration() { EventId = Guid.NewGuid() };
            _candidate.TeachingEventRegistrations.Add(registration);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(mock => mock.Save(_candidate)).Callback(() => _candidate.Id = candidateId);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(registration), Times.Once);
            registration.CandidateId.Should().Be(candidateId);
        }

        [Fact]
        public void Run_WithPhoneCallOnSuccess_SavesPhoneCall()
        {
            var candidateId = Guid.NewGuid();
            var phoneCall = new PhoneCall();
            _candidate.PhoneCall = phoneCall;
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(mock => mock.Save(_candidate)).Callback(() => _candidate.Id = candidateId);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(phoneCall), Times.Once);
            phoneCall.CandidateId.Should().Be(candidateId.ToString());
        }

        [Fact]
        public void Run_WithPhoneCallOnSuccess_IncrementsCallbackBookingQuotaNumberOfBookings()
        {
            var candidateId = Guid.NewGuid();
            var scheduledAt = DateTime.UtcNow.AddDays(3);
            var phoneCall = new PhoneCall() { ScheduledAt = scheduledAt };
            var quota = new CallbackBookingQuota() { StartAt = scheduledAt, NumberOfBookings = 5, Quota = 10 };
            _candidate.PhoneCall = phoneCall;
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(mock => mock.Save(_candidate)).Callback(() => _candidate.Id = candidateId);
            _mockCrm.Setup(mock => mock.GetCallbackBookingQuota(scheduledAt)).Returns(quota);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(quota), Times.Once);
            quota.NumberOfBookings.Should().Be(6);
        }

        [Fact]
        public void Run_WithPhoneCallOnSuccessButMatchingQuotaIsAlreadyFull_DoesNotIncrementsCallbackBookingQuotaNumberOfBookings()
        {
            var candidateId = Guid.NewGuid();
            var scheduledAt = DateTime.UtcNow.AddDays(3);
            var phoneCall = new PhoneCall() { ScheduledAt = scheduledAt };
            var quota = new CallbackBookingQuota() { StartAt = scheduledAt, NumberOfBookings = 5, Quota = 5 };
            _candidate.PhoneCall = phoneCall;
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(mock => mock.Save(_candidate)).Callback(() => _candidate.Id = candidateId);
            _mockCrm.Setup(mock => mock.GetCallbackBookingQuota(scheduledAt)).Returns(quota);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(quota), Times.Never);
            quota.NumberOfBookings.Should().Be(5);
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(23);

            _job.Run(_candidate, null);

            _mockCrm.Verify(mock => mock.Save(_candidate), Times.Never);
            _mockNotifyService.Verify(mock => mock.SendEmailAsync(_candidate.Email,
                NotifyService.CandidateRegistrationFailedEmailTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Started (24/24)");
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Deleted");
        }
    }
}
