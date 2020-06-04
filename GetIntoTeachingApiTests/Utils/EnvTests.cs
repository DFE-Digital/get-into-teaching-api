using System;
using FluentAssertions;
using GetIntoTeachingApi.Utils;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class EnvTests
    {
        [Theory]
        [InlineData("Development", true)]
        [InlineData("Staging", false)]
        [InlineData("Production", false)]
        public void IsDevelopment_TrueOnlyForDevelopment(string environment, bool expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            Env.IsDevelopment.Should().Be(expected);
        }

        [Theory]
        [InlineData("Development", false)]
        [InlineData("Staging", false)]
        [InlineData("Production", true)]
        public void IsProduction_TrueOnlyForProduction(string environment, bool expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            Env.IsProduction.Should().Be(expected);
        }

        [Theory]
        [InlineData("Development", false)]
        [InlineData("Staging", true)]
        [InlineData("Production", false)]
        public void IsStaging_TrueOnlyForStaging(string environment, bool expected)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            Env.IsStaging.Should().Be(expected);
        }
    }
}
