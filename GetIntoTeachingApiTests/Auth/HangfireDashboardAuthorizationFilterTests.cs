using FluentAssertions;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Utils;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Auth
{
    public class HangfireDashboardAuthorizationFilterTests
    {
        private readonly HangfireDashboardAuthorizationFilter _filter;
        private readonly Mock<IEnv> _mockEnv;

        public HangfireDashboardAuthorizationFilterTests()
        {
            _mockEnv = new Mock<IEnv>();
            _filter = new HangfireDashboardAuthorizationFilter(_mockEnv.Object);
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