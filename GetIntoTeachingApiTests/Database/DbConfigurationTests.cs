using System;
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
            _mockEnv.Setup(m => m.DatabaseInstanceName).Returns("get-into-teaching-api-dev-pg-svc-2");
            _mockEnv.Setup(m => m.HangfireInstanceName).Returns("get-into-teaching-api-dev-pg-svc");
            _mockEnv.Setup(m => m.VcapServices).Returns(json);
        }

        [Fact]
        public void DatabaseConnectionString_ReturnsCorrectly()
        {
            var connectionString = DbConfiguration.DatabaseConnectionString(_mockEnv.Object);

            connectionString.Should().Be("Host=host2.coowcrpgh5fz.eu-west-2.rds.amazonaws.com;" +
                                         "Database=rdsbroker_ce514fe0_b017_4caf_8609_c1dc1dec978d;" +
                                         "Username=username2;" +
                                         "Password=password2;" +
                                         "Port=5432;" +
                                         "SSL Mode=Require;" +
                                         "Trust Server Certificate=True");
        }

        [Fact]
        public void HangfireConnectionString_ReturnsCorrectly()
        {
            var connectionString = DbConfiguration.HangfireConnectionString(_mockEnv.Object);

            connectionString.Should().Be("Host=host1.coowcrpgh5fz.eu-west-2.rds.amazonaws.com;" +
                                         "Database=rdsbroker_277c8858_eb3a_427b_99ed_0f4f4171701e;" +
                                         "Username=username1;" +
                                         "Password=password1;" +
                                         "Port=5432;" +
                                         "SSL Mode=Require;" +
                                         "Trust Server Certificate=True;" +
                                         "SearchPath=hangfire");
        }
    }
}
