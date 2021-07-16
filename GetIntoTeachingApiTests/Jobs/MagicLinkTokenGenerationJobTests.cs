using System;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class MagicLinkTokenGenerationJobTests
    {
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<ICandidateMagicLinkTokenService> _mockMagicLinkTokenService;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<ILogger<MagicLinkTokenGenerationJob>> _mockLogger;
        private readonly IMetricService _metrics;
        private readonly MagicLinkTokenGenerationJob _job;

        public MagicLinkTokenGenerationJobTests()
        {
            _mockLogger = new Mock<ILogger<MagicLinkTokenGenerationJob>>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _mockCrm = new Mock<ICrmService>();
            _mockAppSettings = new Mock<IAppSettings>();
            _mockMagicLinkTokenService = new Mock<ICandidateMagicLinkTokenService>();
            _metrics = new MetricService();
            _job = new MagicLinkTokenGenerationJob(
                new Env(),
                _mockJobClient.Object,
                _mockMagicLinkTokenService.Object,
                _mockCrm.Object,
                _mockLogger.Object,
                _metrics,
                _mockAppSettings.Object);
        }

        [Fact]
        public void DisableConcurrentExecutionAttribute()
        {
            var type = typeof(MagicLinkTokenGenerationJob);

            type.GetMethod("Run").Should().BeDecoratedWith<DisableConcurrentExecutionAttribute>();
        }

        [Fact]
        public void Run_UpsertsCandidatesWithMagicLinkTokens()
        {
            var candidate = new Candidate() { Id = Guid.NewGuid(), MagicLinkTokenStatusId = (int)Candidate.MagicLinkTokenStatus.Pending };
            string json = candidate.SerializeChangeTracked();
            _mockCrm.Setup(m => m.GetCandidatesPendingMagicLinkTokenGeneration(500)).Returns(new Candidate[] { candidate });

            _job.Run();

            _mockMagicLinkTokenService.Verify(m => m.GenerateToken(It.Is<Candidate>(c => c.Id == candidate.Id)));

            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run"),
                It.IsAny<EnqueuedState>()));

            _mockLogger.VerifyInformationWasCalled("MagicLinkTokenGenerationJob - Started");
            _mockLogger.VerifyInformationWasCalled("MagicLinkTokenGenerationJob - Processing (1)");
            _mockLogger.VerifyInformationWasCalled("MagicLinkTokenGenerationJob - Succeeded");

            _metrics.MagicLinkTokenGenerationDuration.Count.Should().Be(1);
        }

        [Fact]
        public void Run_WhenCrmIntegrationPaused_DoesNotLoadCandidates()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);

            _job.Run();

            _mockCrm.Verify(m => m.GetCandidatesPendingMagicLinkTokenGeneration(It.IsAny<int>()), Times.Never);
            _mockLogger.VerifyInformationWasCalled("MagicLinkTokenGenerationJob - Skipping (CRM integration paused)");
        }
    }
}