using FluentAssertions;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Utils;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Auth
{
    public class HangfireDashboardEnvironmentAuthorizationFilterTests
    {
        private readonly HangfireDashboardEnvironmentAuthorizationFilter _filter;
        private readonly Mock<IEnv> _mockEnv;

        public HangfireDashboardEnvironmentAuthorizationFilterTests()
        {
            _mockEnv = new Mock<IEnv>();
            _filter = new HangfireDashboardEnvironmentAuthorizationFilter(_mockEnv.Object);
        }

        [Fact]
        public void Authorize_Staging_IsTrue()
        {
            _mockEnv.Setup(m => m.EnvironmentName).Returns("Staging");

            _filter.Authorize(null).Should().BeTrue();
        }

        [Fact]
        public void Authorize_Development_IsTrue()
        {
            _mockEnv.Setup(m => m.EnvironmentName).Returns("Development");

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
            _mockEnv.Setup(m => m.EnvironmentName).Returns(environment);

            _filter.Authorize(null).Should().BeFalse();
        }
    }
}