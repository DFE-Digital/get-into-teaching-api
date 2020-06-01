using System;
using System.IO;
using FluentAssertions;
using GetIntoTeachingApi.Database;
using Xunit;

namespace GetIntoTeachingApiTests.Database
{
    public class DbConfigurationTests
    {
        public DbConfigurationTests()
        {
            using StreamReader reader = new StreamReader("./Fixtures/vcap_services.json");
            var json = reader.ReadToEnd();
            Environment.SetEnvironmentVariable("VCAP_SERVICES", json);
        }

        [Fact]
        public void DatabaseConnectionString_ReturnsCorrectly()
        {
            var connectionString = DbConfiguration.DatabaseConnectionString();

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
            var connectionString = DbConfiguration.HangfireConnectionString();

            connectionString.Should().Be("Host=host1.coowcrpgh5fz.eu-west-2.rds.amazonaws.com;" +
                                         "Database=rdsbroker_277c8858_eb3a_427b_99ed_0f4f4171701e;" +
                                         "Username=username1;" +
                                         "Password=password1;" +
                                         "Port=5432;" +
                                         "SSL Mode=Require;" +
                                         "Trust Server Certificate=True");
        }
    }
}
