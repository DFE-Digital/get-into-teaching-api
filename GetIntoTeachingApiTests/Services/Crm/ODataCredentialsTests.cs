using System;
using FluentAssertions;
using GetIntoTeachingApi.Services.Crm;
using Xunit;

namespace GetIntoTeachingApiTests.Services.Crm
{
    public class ODataCredentialsTests : IDisposable
    {
        private readonly IODataCredentials _credentials;
        private readonly string _previousServiceUrl;
        private readonly string _previousTenantId;
        private readonly string _previousClientId;
        private readonly string _previousSecret;

        public ODataCredentialsTests()
        {
            _previousServiceUrl = Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
            _previousTenantId = Environment.GetEnvironmentVariable("CRM_TENANT_ID");
            _previousClientId = Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
            _previousSecret = Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");

            Environment.SetEnvironmentVariable("CRM_SERVICE_URL", "http://service_url.com");
            Environment.SetEnvironmentVariable("CRM_TENANT_ID", "tenant_id");
            Environment.SetEnvironmentVariable("CRM_CLIENT_ID", "client_id");
            Environment.SetEnvironmentVariable("CRM_CLIENT_SECRET", "client_secret");

            _credentials = new ODataCredentials();
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("CRM_SERVICE_URL", _previousServiceUrl);
            Environment.SetEnvironmentVariable("CRM_TENANT_ID", _previousTenantId);
            Environment.SetEnvironmentVariable("CRM_CLIENT_ID", _previousClientId);
            Environment.SetEnvironmentVariable("CRM_CLIENT_SECRET", _previousSecret);
        }

        [Fact]
        public void ServiceUrl_PopulatesFromEnvironment()
        {
            _credentials.ServiceUrl().Should().Be("http://service_url.com");
        }

        [Fact]
        public void TenantId_PopulatesFromEnvironment()
        {
            _credentials.TenantId().Should().Be("tenant_id");
        }

        [Fact]
        public void ClientId_PopulatesFromEnvironment()
        {
            _credentials.ClientId().Should().Be("client_id");
        }

        [Fact]
        public void Secret_PopulatesFromEnvironment()
        {
            _credentials.Secret().Should().Be("client_secret");
        }

        [Fact]
        public void AuthUrl_PopulatesFromEnvironment()
        {
            _credentials.AuthUrl().Should().Be("https://login.microsoftonline.com/tenant_id");
        }

        [Fact]
        public void ODataServiceUri_PopulatesFromEnvironment()
        {
            _credentials.ODataServiceUri().Should().Be(new Uri($"{_credentials.ServiceUrl()}/api/data/v9.1"));
        }
    }
}
