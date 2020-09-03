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
        private const string CRM_SERVICE_HOST = "https://gitis-dev.api.crm4.dynamics.com";
        // private const string CRM_CLIENT_ID = "123456";
        private const string CRM_CLIENT_ID = "965b7a3a-dc0a-4f4f-b75c-14ad6dbc57f2";
        // private const string CRM_CLIENT_SECRET = "123456";
        private const string CRM_CLIENT_SECRET = "3Q_bZ0epYn8oB._QtLYshtPUX5M8~Zhe-~";

        private readonly string _crmServiceUrl = $"{CRM_SERVICE_HOST}";

        // ReSharper disable once UnusedMember.Global
        public ContractTestFixture()
            : this(Path.Combine(""))
        {
        }

        private ContractTestFixture(string relativeTargetProjectParentDir)
        {
            ContractTestEnvironment.Setup(SHARED_SECRET, _crmServiceUrl, CRM_CLIENT_ID, CRM_CLIENT_SECRET);
            
            Server = new ServerUnderTest<TStartup>(relativeTargetProjectParentDir);
            Client = CreateHttpClientForTestServer(Server);
        }

        public ServerUnderTest Server { get; }
        public HttpClient Client { get; }

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