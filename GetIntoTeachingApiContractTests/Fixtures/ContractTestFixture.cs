using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using GetIntoTeachingApiContractTests.Environments;
using GetIntoTeachingApiContractTests.Servers;
using Microsoft.AspNetCore.TestHost;
using WireMock.Server;

namespace GetIntoTeachingApiContractTests.Fixtures
{
    public class ContractTestFixture<TStartup> : IDisposable
    {
        private const string SHARED_SECRET = "shared_secret";
        private const string CRM_SERVICE_URL = "https://localhost:8080/Test";
        private const string CRM_CLIENT_ID = "123456";
        private const string CRM_CLIENT_SECRET = "123456";

        // ReSharper disable once UnusedMember.Global
        public ContractTestFixture()
            : this(Path.Combine(""))
        {
        }

        private ContractTestFixture(string relativeTargetProjectParentDir)
        {
            ContractTestEnvironment.Setup(SHARED_SECRET, CRM_SERVICE_URL, CRM_CLIENT_ID, CRM_CLIENT_SECRET);
            
            CrmServer = new MockCrmServer(CRM_SERVICE_URL);
            Server = new ServerUnderTest<TStartup>(relativeTargetProjectParentDir);
            Client = CreateHttpClientForTestServer(Server);
        }

        public TestServer Server { get; }
        public WireMockServer CrmServer { get; }
        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
            
            Server.Dispose();
            
            CrmServer.Stop();
            CrmServer.Dispose();
            
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