using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class UpsertCandidateJobTests
    {
        private readonly Mock<IPerformContextAdapter> _mockContext;
        private readonly Mock<ICandidateUpserter> _mockUpserter;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<IDatabase> _mockDatabase;
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

            _mockDatabase = new Mock<IDatabase>();
            var mockRedis = new Mock<IRedisService>();
            mockRedis.Setup(m => m.Database).Returns(_mockDatabase.Object);

            _job = new UpsertCandidateJob(new Env(), mockRedis.Object, _mockUpserter.Object, _mockNotifyService.Object,
                _mockContext.Object, _metrics, _mockLogger.Object, _mockAppSettings.Object);

            _metrics.HangfireJobQueueDuration.RemoveLabelled(new[] { "UpsertCandidateJob" });
            _mockContext.Setup(m => m.GetJobCreatedAt(null)).Returns(DateTime.UtcNow.AddDays(-1));

            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
        }

        [Fact]
        public void Run_OnSuccess_UpsertsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            var key = $"base_job.UpsertCandidateJob.{Signature(_candidate)}";
            _mockDatabase.Setup(d => d.KeyExists(key, CommandFlags.None)).Returns(false);

            var json = _candidate.SerializeChangeTracked();
            _job.Run(json, null);

            _mockUpserter.Verify(mock => mock.Upsert(It.Is<Candidate>(c => IsMatch(_candidate, c))), Times.Once);
            _mockLogger.VerifyInformationWasCalled("UpsertCandidateJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled($"UpsertCandidateJob - Payload {Redactor.RedactJson(json)}");
            _mockLogger.VerifyInformationWasCalled($"UpsertCandidateJob - Succeeded - {_candidate.Id}");
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertCandidateJob" }).Count.Should().Be(1);
            _mockDatabase.Verify(m => m.StringSet(key, true, TimeSpan.FromSeconds(5), When.Always, CommandFlags.None));
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
        public void Run_WhenCrmIntegrationPaused_Aborts()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);
            var key = $"base_job.UpsertCandidateJob.{Signature(_candidate)}";
            _mockDatabase.Setup(d => d.KeyExists(key, CommandFlags.None)).Returns(false);

            var json = _candidate.SerializeChangeTracked();
            Action action = () => _job.Run(json, null);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("UpsertCandidateJob - Aborting (CRM integration paused).");
        }

        [Fact]
        public void Run_WhenTheJobHasBeenDuplicated_Deduplicates()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            var key = $"base_job.UpsertCandidateJob.{Signature(_candidate)}";
            _mockDatabase.Setup(d => d.KeyExists(key, CommandFlags.None)).Returns(true);

            var json = _candidate.SerializeChangeTracked();
            _job.Run(json, null);

            _mockUpserter.Verify(mock => mock.Upsert(It.Is<Candidate>(c => IsMatch(_candidate, c))), Times.Never);
            _mockDatabase.Verify(m => m.StringSet(key, true, TimeSpan.FromSeconds(5), When.Always, CommandFlags.None), Times.Never);
        }

        [Fact]
        public void Run_WhenTheJobIsRetrying_DoesNotDeduplicate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(1);

            var json = _candidate.SerializeChangeTracked();
            _job.Run(json, null);

            _mockUpserter.Verify(mock => mock.Upsert(It.Is<Candidate>(c => IsMatch(_candidate, c))), Times.Once);
            var key = $"base_job.UpsertCandidateJob.{Signature(_candidate)}";
            _mockDatabase.Verify(m => m.KeyExists(key, CommandFlags.None), Times.Never);

        }

        private static bool IsMatch(object objectA, object objectB)
        {
            objectA.Should().BeEquivalentTo(objectB);
            return true;
        }

        private static string Signature(Candidate candidate)
        {
            var changedProperties = "MergedIsNewRegistrantTeachingEventRegistrationsQualificationsPastTeachingPositionsApplicationFormsSchoolExperiencesIdEmail";
            return $"{candidate.Id}-{candidate.Email}-{changedProperties}";
        }
    }
}
