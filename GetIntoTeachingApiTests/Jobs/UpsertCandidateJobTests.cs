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
        private readonly Mock<ICandidateUpserter> _mockUpserter;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Candidate _candidate;
        private readonly IMetricService _metrics;
        private readonly UpsertCandidateJob _job;
        private readonly Mock<ILogger<UpsertCandidateJob>> _mockLogger;

        public UpsertCandidateJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockUpserter = new Mock<ICandidateUpserter>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockLogger = new Mock<ILogger<UpsertCandidateJob>>();
            _mockAppSettings = new Mock<IAppSettings>();
            _metrics = new MetricService();
            _candidate = new Candidate() { Id = Guid.NewGuid(), Email = "test@test.com" };
            _job = new UpsertCandidateJob(new Env(), _mockUpserter.Object, _mockNotifyService.Object,
                _mockContext.Object, _metrics, _mockLogger.Object, _mockAppSettings.Object);

            _metrics.HangfireJobQueueDuration.RemoveLabelled(new[] { "UpsertCandidateJob" });
            _mockContext.Setup(m => m.GetJobCreatedAt(null)).Returns(DateTime.UtcNow.AddDays(-1));

            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
        }

        [Fact]
        public void Run_OnSuccess_UpsertsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            var json = _candidate.SerializeChangeTracked();
            _job.Run(json, null);

            _mockUpserter.Verify(mock => mock.Upsert(It.Is<Candidate>(c => IsMatch(_candidate, c))), Times.Once);
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled($"UpsertCandidateJob - Payload {Redactor.RedactJson(json)}");
            _mockLogger.VerifyInformationWasCalled($"UpsertCandidateJob - Succeeded - {_candidate.Id}");
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertCandidateJob" }).Count.Should().Be(1);
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(23);

            _job.Run(_candidate.SerializeChangeTracked(), null);

            _mockUpserter.Verify(mock => mock.Upsert(It.IsAny<Candidate>()), Times.Never);
            _mockNotifyService.Verify(mock => mock.SendEmailAsync(_candidate.Email,
                NotifyService.CandidateRegistrationFailedEmailTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Started (24/24)");
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Deleted");
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertCandidateJob" }).Count.Should().Be(1);
        }

        [Fact]
        public void Run_OnError_LogsAndReRaises()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            var message = "boom!";
            var exception = new Exception(message);
            _mockUpserter.Setup(m => m.Upsert(It.IsAny<Candidate>())).Throws(exception);

            _job.Invoking(j => j.Run(_candidate.SerializeChangeTracked(), null))
                .Should().Throw<Exception>().WithMessage(message);

            _mockLogger.VerifyErrorWasCalled($"UpsertCandidateJob - Exception - System.Exception: {message}");
        }

        [Fact]
        public void Run_WhenCrmIntegrationPaused_Aborts()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);

            var json = _candidate.SerializeChangeTracked();
            Action action = () => _job.Run(json, null);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("UpsertCandidateJob - Aborting (CRM integration paused).");
        }

        private bool IsMatch(object objectA, object objectB)
        {
            objectA.Should().BeEquivalentTo(objectB);
            return true;
        }
    }
}
