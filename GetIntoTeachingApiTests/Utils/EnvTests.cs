using System;
using FluentAssertions;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    // We're changing the environment in these tests.
    [Collection(nameof(NotThreadSafeResourceCollection))]
    public class EnvTests : IDisposable
    {
        private readonly string _previousEnvironment;
        private readonly string _previousCfInstanceIndex;
        private readonly IEnv _env;

        public EnvTests()
        {
            _env = new Env();
            _previousEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _previousCfInstanceIndex = Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX");
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", _previousEnvironment);
            Environment.SetEnvironmentVariable("CF_INSTANCE_INDEX", _previousCfInstanceIndex);
        }

        [Theory]
        [InlineData("0", true)]
        [InlineData("1", false)]
        [InlineData("10", false)]
        public void ExportHangfireToPrometheus_TrueOnlyForFirstInstance(string instance, bool expected)
        {
            Environment.SetEnvironmentVariable("CF_INSTANCE_INDEX", instance);

            _env.ExportHangireToPrometheus.Should().Be(expected);
        }

        [Theory]
        [InlineData("Development", true)]
        [InlineData("Staging", false)]
        [InlineData("Production", false)]
        public void IsDevelopment_TrueOnlyForDevelopment(string environment, bool expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            _env.IsDevelopment.Should().Be(expected);
        }

        [Theory]
        [InlineData("Development", false)]
        [InlineData("Staging", false)]
        [InlineData("Production", true)]
        public void IsProduction_TrueOnlyForProduction(string environment, bool expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            _env.IsProduction.Should().Be(expected);
        }

        [Theory]
        [InlineData("Development", false)]
        [InlineData("Staging", true)]
        [InlineData("Production", false)]
        public void IsStaging_TrueOnlyForStaging(string environment, bool expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            _env.IsStaging.Should().Be(expected);
        }
    }
}
