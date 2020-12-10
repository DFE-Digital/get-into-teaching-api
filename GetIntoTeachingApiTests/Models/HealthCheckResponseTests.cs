using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class HealthCheckResponseTests
    {
        [Fact]
        public void Loggable_IsPresent()
        {
            typeof(HealthCheckResponse).Should().BeDecoratedWith<LoggableAttribute>();
        }

        [Theory]
        [InlineData(true, true, true, true, true, "healthy")]
        [InlineData(true, true, true, false, true, "degraded")]
        [InlineData(true, true, true, false, false, "degraded")]
        [InlineData(true, true, true, true, false, "degraded")]
        [InlineData(true, true, false, true, true, "degraded")]
        [InlineData(false, true, true, true, true, "unhealthy")]
        [InlineData(true, false, true, true, true, "unhealthy")]
        [InlineData(false, false, true, true, true, "unhealthy")]
        [InlineData(true, false, false, true, true, "unhealthy")]
        [InlineData(false, false, false, true, false, "unhealthy")]
        [InlineData(false, false, false, false, false, "unhealthy")]
        public void Status_ReturnsCorrectly(bool database, bool hangfire, bool redis, bool crm, bool notify, string expectedStatus)
        {
            var databaseStatus = database ? HealthCheckResponse.StatusOk : "error";
            var hangfireStatus = hangfire ? HealthCheckResponse.StatusOk : "error";
            var crmStatus = crm ? HealthCheckResponse.StatusOk : "error";
            var notifyStatus = notify ? HealthCheckResponse.StatusOk : "error";
            var redisStatus = redis ? HealthCheckResponse.StatusOk : "error";

            var health = new HealthCheckResponse()
            {
                Crm = crmStatus,
                Database = databaseStatus,
                Notify = notifyStatus,
                Hangfire = hangfireStatus,
                Redis = redisStatus,
            };

            health.Status.Should().Be(expectedStatus);
        }
    }
}
