using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using GetIntoTeachingApiContractTests.Environments;
using GetIntoTeachingApiContractTests.Servers;
using Microsoft.AspNetCore.TestHost;

namespace GetIntoTeachingApiContractTests.Fixtures
{
    public class ContractTestFixture<TStartup> : IDisposable
    {
        private const string SHARED_SECRET = "shared_secret";
        
        private const string CRM_SERVICE_HOST = "https://gitis-mock.api.crm4.dynamics.com";
        private const string CRM_CLIENT_ID = "123456";
        private const string CRM_CLIENT_SECRET = "123456";
        
        private const bool ALLOW_PASSTHROUGH_TO_CRM = true;

        private readonly string _crmServiceUrl = $"{CRM_SERVICE_HOST}";

        // ReSharper disable once UnusedMember.Global
        public ContractTestFixture()
            : this(Path.Combine(""))
        {
        }

        private ContractTestFixture(string relativeTargetProjectParentDir)
        {
            ContractTestEnvironment.Setup(SHARED_SECRET, _crmServiceUrl, CRM_CLIENT_ID, CRM_CLIENT_SECRET);
            
            Server = new ServerUnderTest<TStartup>(relativeTargetProjectParentDir, ALLOW_PASSTHROUGH_TO_CRM);
            Client = CreateHttpClientForTestServer(Server);
            ContractDataPath = Path.Combine(ServerUnderTest.ContentRoot, "../GetIntoTeachingApiContractTests/contracts/");

        }

        public ServerUnderTest Server { get; }
        public HttpClient Client { get; }
        public string ContractDataPath { get; }

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
            
            Environment.SetEnvironmentVariable("SHARED_SECRET", null);
        }

        private HttpClient CreateHttpClientForTestServer(TestServer server)
        {
            // Add configuration for client
            var client = server.CreateClient();
            client.BaseAddress = new Uri(Server.BaseAddress.AbsoluteUri);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(SHARED_SECRET);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}