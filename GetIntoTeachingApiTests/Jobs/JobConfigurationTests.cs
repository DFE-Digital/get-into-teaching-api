using System;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Utils;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class JobConfigurationTests
    {
        private readonly Mock<IEnv> _mockEnv;

        public JobConfigurationTests()
        {
            _mockEnv = new Mock<IEnv>();
        }

        [Theory]
        [InlineData(true, 5)]
        [InlineData(false, 24)]
        public void Attempts_WithEnvironment_ReturnsCorrectly(bool development, int expected)
        {
            _mockEnv.Setup(m => m.IsDevelopment).Returns(development);

            JobConfiguration.Attempts(_mockEnv.Object).Should().Be(expected);
        }

        [Theory]
        [InlineData(true, 60)]
        [InlineData(false, 3600)]
        public void RetryIntervalInSeconds_WithEnvironment_ReturnsCorrectly(bool development, int expected)
        {
            _mockEnv.Setup(m => m.IsDevelopment).Returns(development);

            JobConfiguration.RetryIntervalInSeconds(_mockEnv.Object).Should().Be(expected);
        }

        [Theory]
        [InlineData(true, 24)]
        [InlineData(false, 24)]
        public void ExpirationTimeout_WithEnvironment_ReturnsCorrectly(bool development, int expectedInHours)
        {
            _mockEnv.Setup(m => m.IsDevelopment).Returns(development);

            JobConfiguration.ExpirationTimeout.Should().Be(TimeSpan.FromHours(expectedInHours));
        }
    }
}
