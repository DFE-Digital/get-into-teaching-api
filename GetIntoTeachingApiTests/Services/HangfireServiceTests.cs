using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Hangfire;
using Hangfire.Storage.Monitoring;
using Hangfire.Storage;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class HangfireServiceTests
    {
        private readonly Mock<IStorageConnection> _mockStorageConnection;
        private readonly Mock<IMonitoringApi> _mockMonitoringApi;
        private readonly HangfireService _hangfire;

        public HangfireServiceTests()
        {
            _mockStorageConnection = new Mock<IStorageConnection>();
            _mockMonitoringApi = new Mock<IMonitoringApi>();

            var mockStorage = new Mock<JobStorage>();
            mockStorage.Setup(x => x.GetConnection()).Returns(_mockStorageConnection.Object);
            mockStorage.Setup(x => x.GetMonitoringApi()).Returns(_mockMonitoringApi.Object);

            _hangfire = new HangfireService(mockStorage.Object);
        }

        [Fact]
        public void CheckStatus_WhenHealthy_ReturnsOk()
        {
            var servers = new List<ServerDto>() { new ServerDto() { Queues = new[] { "Default" } } };
            _mockMonitoringApi.Setup(m => m.Servers()).Returns(servers);

            _hangfire.CheckStatus().Should().Be(HealthCheckResponse.StatusOk);
        }

        [Fact]
        public void CheckStatus_WhenUnhealthy_ReturnsError()
        {
            _mockMonitoringApi.Setup(m => m.Servers()).Returns(new List<ServerDto>());

            _hangfire.CheckStatus().Should().Contain("No workers are processing the Default queue!");
        }
    }
}
