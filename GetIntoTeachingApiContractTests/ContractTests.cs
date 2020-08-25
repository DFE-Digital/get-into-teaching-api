using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GetIntoTeachingApi;
using GetIntoTeachingApiContractTests.Fixtures;
using GetIntoTeachingApiContractTests.Helpers;
using Microsoft.AspNetCore.TestHost;
using RestSharp;
using WireMock.Logging;
using WireMock.Server;
using Xunit;

namespace GetIntoTeachingApiContractTests
{
    public class ContractTests : IClassFixture<ContractTestFixture<Startup>>
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;
        private readonly WireMockServer _crmServer;

        public ContractTests(ContractTestFixture<Startup> fixture)
        {
            _client = fixture.Client;
            _server = fixture.Server;
            _crmServer = fixture.CrmServer;
        }

        [Fact]
        public void WithRestClient()
        {
            var client = new RestClient("https://www.bbc.co.uk");
            var clientRequest = new RestRequest("/");
            var clientResponse = client.Get(clientRequest);

            Assert.True(clientResponse.IsSuccessful, "Response should be successful");
        }

        [Fact]
        public async Task CanCheckHealth()
        {
            var url = "/api/operations/health_check";
            

            // Act
            var response = await _client.GetAsync(url);
            var message = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ThroughApiService()
        {
            // Arrange
            var request = new
            {
                Url = "/api/teacher_training_adviser/candidates",
                Body = new
                {
                    countryId = "0df4c2e6-74f9-e811-a97a-000d3a2760f2",
                    preferredEducationPhaseId = 222750001,
                    firstName = "Lino",
                    lastName = "Hayes",
                    email = "scottie.towne@example.net",
                    teacherId = "123456",
                    subjectTaughtId = "942655a1-2afa-e811-a981-000d3a276620",
                    preferredTeachingSubjectId = "ac2655a1-2afa-e811-a981-000d3a276620",
                    dateOfBirth = "1990-03-26T00:00:00.000+00:00",
                    addressLine1 = "3702 Stephaine Roads",
                    addressLine2 = "Suite 141",
                    addressCity = "Reichelmouth",
                    addressPostcode = "YN0 1BP",
                    telephone = "0119 608 9170",
                    acceptedPolicyId = "0a203956-e935-ea11-a813-000d3a44a8e9"
                }
            };

            /*_crmServer
                .Given(Request.Create()
                    .WithPath("/XRMServices/2011/Organization.svc/web")
                    .UsingPost()
                    .WithHeader()
                    .WithBody(new XPathMatcher("/")));*/

            // Act
            var response = await _client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var message = await response.Content.ReadAsStringAsync();

            LogEntry crmRequest = null;
            do
            {
                crmRequest = _crmServer.LogEntries
                    .FirstOrDefault(entry =>entry.RequestMessage.Body.Contains("Candidate"));
            } while (crmRequest == null);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
