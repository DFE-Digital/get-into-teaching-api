using System;
using System.Linq.Expressions;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Hangfire;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class HangfireServiceTests
    {
        private readonly Mock<ILogger<HangfireService>> _mockLogger;

        public HangfireServiceTests()
        {
            _mockLogger = new Mock<ILogger<HangfireService>>();
        }

        [Fact]
        public void CheckStatus_WhenHealthy_ReturnsOk()
        {
            var mockJobClient = new Mock<IBackgroundJobClient>();
            var hangfire = new HangfireService(mockJobClient.Object, _mockLogger.Object);

            hangfire.CheckStatus().Should().Be(HealthCheckResponse.StatusOk);
        }

        [Fact]
        public void CheckStatus_WhenUnhealthy_ReturnsError()
        {
            var hangfire = new HangfireService(null, _mockLogger.Object);

            hangfire.CheckStatus().Should().Contain("Value cannot be null");
        }
    }
}
