using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Prometheus;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class CrmSyncJobTests
    {
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IStore> _mockStore;
        private readonly Mock<ILogger<CrmSyncJob>> _mockLogger;
        private readonly IMetricService _metrics;
        private readonly CrmSyncJob _job;

        public CrmSyncJobTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _mockLogger = new Mock<ILogger<CrmSyncJob>>();
            _mockStore = new Mock<IStore>();
            _metrics = new MetricService();
            _job = new CrmSyncJob(_mockCrm.Object, _mockStore.Object, _mockLogger.Object, _metrics);
        }

        [Fact]
        public async void RunAsync_CallsSync()
        {
            await _job.RunAsync();

            _mockStore.Verify(mock => mock.SyncAsync(_mockCrm.Object), Times.Once);
            _mockLogger.VerifyInformationWasCalled("CrmSyncJob - Started");
            _mockLogger.VerifyInformationWasCalled("CrmSyncJob - Succeeded");
            _metrics.CrmSyncDuration.Count.Should().Be(1);
        }
    }
}