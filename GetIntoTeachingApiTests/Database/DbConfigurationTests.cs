using System.IO;
using FluentAssertions;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Utils;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Database
{
    public class DbConfigurationTests
    {
        private readonly Mock<IEnv> _mockEnv;

        public DbConfigurationTests()
        {
            using var reader = new StreamReader("./Fixtures/vcap_services.json");
            var json = reader.ReadToEnd();

            _mockEnv = new Mock<IEnv>();
            _mockEnv.Setup(m => m.DatabaseInstanceName).Returns("database");
            _mockEnv.Setup(m => m.HangfireInstanceName).Returns("hangfire");
            _mockEnv.Setup(m => m.VcapServices).Returns(json);
        }

        [Fact]
        public void DatabaseConnectionString_ReturnsCorrectly()
        {
            var connectionString = DbConfiguration.DatabaseConnectionString(_mockEnv.Object);

            connectionString.Should().Be("Host=host.com;" +
                                         "Database=database;" +
                                         "Username=username2;" +
                                         "Password=password2;" +
                                         "Port=1234;" +
                                         "SSL Mode=Require;" +
                                         "Trust Server Certificate=True");
        }

        [Fact]
        public void HangfireConnectionString_ReturnsCorrectly()
        {
            var connectionString = DbConfiguration.HangfireConnectionString(_mockEnv.Object);

            connectionString.Should().Be("Host=host.com;" +
                                         "Database=hangfire;" +
                                         "Username=username1;" +
                                         "Password=password1;" +
                                         "Port=1234;" +
                                         "SSL Mode=Require;" +
                                         "Trust Server Certificate=True;" +
                                         "SearchPath=hangfire");
        }
    }
}
