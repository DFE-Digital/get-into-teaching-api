using System;
using FluentAssertions;
using GetIntoTeachingApi.Auth;
using Xunit;

namespace GetIntoTeachingApiTests.Auth
{
    public class HangfireDashboardAuthorizationFilterTests : IDisposable
    {
        private readonly HangfireDashboardAuthroizationFilter _filter;
        private readonly string _previousEnvironment;

        public HangfireDashboardAuthorizationFilterTests()
        {
            _filter = new HangfireDashboardAuthroizationFilter();
            _previousEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }
        public void Dispose()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", _previousEnvironment);
        }

        [Fact]
        public void Authorize_Staging_IsTrue()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Staging");

            _filter.Authorize(null).Should().BeTrue();
        }

        [Fact]
        public void Authorize_Development_IsTrue()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            _filter.Authorize(null).Should().BeTrue();
        }

        [Theory]
        [InlineData("Production")]
        [InlineData("Development1")]
        [InlineData("prod")]
        [InlineData("")]
        [InlineData(null)]
        public void Authorize_Other_IsFalse(string environment)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            _filter.Authorize(null).Should().BeFalse();
        }
    }
}