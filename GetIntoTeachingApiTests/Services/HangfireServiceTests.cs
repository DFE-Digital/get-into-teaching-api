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
        [Fact]
        public void CheckStatus_WhenHealthy_ReturnsOk()
        {
            var mockJobClient = new Mock<IBackgroundJobClient>();
            var hangfire = new HangfireService(mockJobClient.Object);

            hangfire.CheckStatus().Should().Be(HealthCheckResponse.StatusOk);
        }

        [Fact]
        public void CheckStatus_WhenUnhealthy_ReturnsError()
        {
            var hangfire = new HangfireService(null);

            hangfire.CheckStatus().Should().Contain("Value cannot be null");
        }
    }
}
