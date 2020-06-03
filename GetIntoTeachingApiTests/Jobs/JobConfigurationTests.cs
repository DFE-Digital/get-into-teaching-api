using System;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class JobConfigurationTests : IDisposable
    {
        private static string _previousEnvironment;

        public JobConfigurationTests()
        {
            _previousEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }

        public void Dispose()
        {  
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", _previousEnvironment);
        }

        [Theory]
        [InlineData("Development", 5)]
        [InlineData("Production", 24)]
        [InlineData("Staging", 24)]
        public void Attempts_WithEnvironment_ReturnsCorrectly(string environment, int expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            JobConfiguration.Attempts.Should().Be(expected);
        }

        [Theory]
        [InlineData("Development", 60)]
        [InlineData("Production", 3600)]
        [InlineData("Staging", 3600)]
        public void RetryIntervalInSeconds_WithEnvironment_ReturnsCorrectly(string environment, int expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            JobConfiguration.RetryIntervalInSeconds.Should().Be(expected);
        }

        [Theory]
        [InlineData("Development", 24)]
        [InlineData("Production", 24)]
        [InlineData("Staging", 24)]
        public void ExpirationTimeout_WithEnvironment_ReturnsCorrectly(string environment, int expectedInHours)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            JobConfiguration.ExpirationTimeout.Should().Be(TimeSpan.FromHours(expectedInHours));
        }
    }
}
