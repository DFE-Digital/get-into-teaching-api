using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class CrmSyncJobTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<ILogger<CrmSyncJob>> _mockLogger;
        private readonly IMetricService _metrics;
        private readonly CrmSyncJob _job;

        public CrmSyncJobTests()
        {
            _mockLogger = new Mock<ILogger<CrmSyncJob>>();
            _mockStore = new Mock<IStore>();
            _mockAppSettings = new Mock<IAppSettings>();
            _metrics = new MetricService();
            _job = new CrmSyncJob(new Env(), _mockStore.Object, _mockLogger.Object, _metrics, _mockAppSettings.Object);
        }

        [Fact]
        public void DisableConcurrentExecutionAttribute()
        {
            var type = typeof(CrmSyncJob);

            type.GetMethod("RunAsync").Should().BeDecoratedWith<DisableConcurrentExecutionAttribute>();
        }

        [Fact]
        public async void RunAsync_CallsSync()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);

            await _job.RunAsync();

            _mockStore.Verify(mock => mock.SyncAsync(), Times.Once);
            _mockLogger.VerifyInformationWasCalled("CrmSyncJob - Started");
            _mockLogger.VerifyInformationWasCalled("CrmSyncJob - Succeeded");
            _metrics.CrmSyncDuration.Count.Should().Be(1);
        }

        [Fact]
        public async void RunAsync_WhenCrmIntegrationPaused_DoesNotCallSync()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);

            await _job.RunAsync();

            _mockStore.Verify(mock => mock.SyncAsync(), Times.Never);
            _mockLogger.VerifyInformationWasCalled("CrmSyncJob - Skipping (CRM integration paused)");
        }
    }
}