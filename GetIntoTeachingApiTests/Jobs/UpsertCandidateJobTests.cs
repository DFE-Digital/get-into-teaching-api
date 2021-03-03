using System;
using System.Collections.Generic;
using System.Text.Json;
using Dahomey.Json;
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
        private readonly JsonSerializerOptions _options;
        private readonly IMetricService _metrics;
        private readonly UpsertCandidateJob _job;
        private readonly Mock<ILogger<UpsertCandidateJob>> _mockLogger;

        public UpsertCandidateJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockLogger = new Mock<ILogger<UpsertCandidateJob>>();
            _options = new JsonSerializerOptions() { IgnoreNullValues = true };
            _options.SetupExtensions();
            _metrics = new MetricService();
            _candidate = new Candidate() { Id = Guid.NewGuid(), Email = "test@test.com" };
            _job = new UpsertCandidateJob(new Env(), _mockCrm.Object, _mockNotifyService.Object,
                _mockContext.Object, _metrics, _mockLogger.Object);

            _metrics.HangfireJobQueueDuration.RemoveLabelled(new[] { "UpsertCandidateJob" });
            _mockContext.Setup(m => m.GetJobCreatedAt(null)).Returns(DateTime.UtcNow.AddDays(-1));
        }

        [Fact]
        public void Run_OnSuccess_SavesCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(JsonSerializer.Serialize(_candidate, _options), null);

            _mockCrm.Verify(mock => mock.Save(It.Is<Candidate>(c => IsMatch(_candidate, c))), Times.Once);
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled($"UpsertCandidateJob - Succeeded - {_candidate.Id}");
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertCandidateJob" }).Count.Should().Be(1);
        }

        [Fact]
        public void Run_WithTeachingEventRegistrationsOnSuccess_SavesTeachingEventRegistrations()
        {
            var candidateId = Guid.NewGuid();
            var registration = new TeachingEventRegistration() { EventId = Guid.NewGuid() };
            _candidate.TeachingEventRegistrations.Add(registration);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);

            _job.Run(JsonSerializer.Serialize(_candidate, _options), null);

            registration.CandidateId = candidateId;
            _mockCrm.Verify(mock => mock.Save(It.Is<TeachingEventRegistration>(r => IsMatch(registration, r))), Times.Once);
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertCandidateJob" }).Count.Should().Be(1);
        }

        [Fact]
        public void Run_WithPhoneCallOnSuccess_SavesPhoneCallAndIncrementsCallbackBookingQuotaNumberOfBookings()
        {
            var candidateId = Guid.NewGuid();
            var scheduledAt = DateTime.UtcNow.AddDays(3);
            var phoneCall = new PhoneCall() { ScheduledAt = scheduledAt };
            _candidate.PhoneCall = phoneCall;
            var quota = new CallbackBookingQuota() { StartAt = scheduledAt, NumberOfBookings = 5, Quota = 10 };
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);
            _mockCrm.Setup(mock => mock.GetCallbackBookingQuota(scheduledAt)).Returns(quota);

            _job.Run(JsonSerializer.Serialize(_candidate, _options), null);

            phoneCall.CandidateId = candidateId.ToString();
            _mockCrm.Verify(mock => mock.Save(It.Is<PhoneCall>(p => IsMatch(phoneCall, p))), Times.Once);
            _mockCrm.Verify(mock => mock.Save(It.Is<CallbackBookingQuota>(q => IsMatch(quota, q))), Times.Once);
            quota.NumberOfBookings.Should().Be(6);
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertCandidateJob" }).Count.Should().Be(1);
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
            _mockCrm.Setup(mock => mock.Save(It.IsAny<Candidate>())).Callback<BaseModel>(c => c.Id = candidateId);
            _mockCrm.Setup(mock => mock.GetCallbackBookingQuota(scheduledAt)).Returns(quota);

            _job.Run(JsonSerializer.Serialize(_candidate, _options), null);

            _mockCrm.Verify(mock => mock.Save(It.IsAny<CallbackBookingQuota>()), Times.Never);
            quota.NumberOfBookings.Should().Be(5);
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertCandidateJob" }).Count.Should().Be(1);
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(23);

            _job.Run(JsonSerializer.Serialize(_candidate, _options), null);

            _mockCrm.Verify(mock => mock.Save(It.IsAny<Candidate>()), Times.Never);
            _mockNotifyService.Verify(mock => mock.SendEmailAsync(_candidate.Email,
                NotifyService.CandidateRegistrationFailedEmailTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Started (24/24)");
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Deleted");
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertCandidateJob" }).Count.Should().Be(1);
        }

        private bool IsMatch(object objectA, object objectB)
        {
            objectA.Should().BeEquivalentTo(objectB);
            return true;
        }
    }
}
