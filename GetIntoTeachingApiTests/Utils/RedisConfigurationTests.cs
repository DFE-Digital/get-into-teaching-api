using System.IO;
using FluentAssertions;
using GetIntoTeachingApi.Utils;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class RedisConfigurationTests
    {
        private readonly Mock<IEnv> _mockEnv;

        public RedisConfigurationTests()
        {
            using var reader = new StreamReader("./Fixtures/vcap_services.json");
            var json = reader.ReadToEnd();

            _mockEnv = new Mock<IEnv>();
            _mockEnv.Setup(m => m.VcapServices).Returns(json);
        }

        [Fact]
        public void ConfigurationOptions_ReturnsCorrectly()
        {
            var options = RedisConfiguration.ConfigurationOptions(_mockEnv.Object);
            var endpoint = options.EndPoints[0];

            endpoint.ToString().Should().Contain("host.com:1234");
            options.Ssl.Should().BeTrue();
            options.Password.Should().Be("password");
        }
    }
}
