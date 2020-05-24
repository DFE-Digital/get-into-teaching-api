using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services.Crm;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Microsoft.Xrm.Sdk;
using Moq;
using Newtonsoft.Json.Linq;
using Simple.OData.Client;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace GetIntoTeachingApiTests.Services.Crm
{
    public class WebApiClientTests
    {
        private readonly WebApiClient _client;
        private readonly Mock<IODataClient> _mockODataClient;

        public WebApiClientTests()
        {
            var cache = new WebApiClientCache(new Mock<ILogger<WebApiClientCache>>().Object);
            _mockODataClient = new Mock<IODataClient>();
            _client = new WebApiClient(cache, _mockODataClient.Object);
        }

        [Fact]
        public async void CreateODataClient_ReturnsCorrectlyConfiguredClient()
        {
            const string token = "abc123";
            var metadata = await File.ReadAllTextAsync("./Fixtures/metadata.xml");

            var server = WireMockServer.Start();

            var mockCredentials = new Mock<IODataCredentials>();
            mockCredentials.Setup(m => m.ServiceUrl()).Returns($"http://localhost:{server.Ports.First()}");

            var mockTokenProvider = new Mock<IAccessTokenProvider>();
            mockTokenProvider.Setup(m => m.GetAccessTokenAsync(mockCredentials.Object)).ReturnsAsync(token);

            var metadataRequest = Request.Create().WithPath("/api/data/v9.1/$metadata")
                        .WithHeader("Authorization", "Bearer abc123")
                        .WithHeader("OData-MaxVersion", "4.0")
                        .WithHeader("OData-Version", "4.0");

            server
                .Given(metadataRequest)
                .RespondWith(Response.Create().WithStatusCode(200)
                .WithHeader("Content-Type", "application/xml").WithBody(metadata));

            WebApiClient.CreateODataClient(mockCredentials.Object, mockTokenProvider.Object);

            server.FindLogEntries(metadataRequest).Count().Should().Be(1);

            server.Stop();
        }

        [Fact]
        public async void GetLookupItems_ReturnsAll()
        {
            _mockODataClient.Setup(m => m.For("dfe_countries").Select("dfe_countryid", "dfe_name").FindEntriesAsync())
                .ReturnsAsync(MockCountries());

            var result = await _client.GetLookupItems(Lookup.Country);

            result.Select(country => country.Value).Should()
                .BeEquivalentTo(MockCountries().Select(c => c["dfe_name"]), 
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async void GetLookupItems_IsCached()
        {
            _mockODataClient.Setup(m => m.For("dfe_countries").Select("dfe_countryid", "dfe_name").FindEntriesAsync())
                .ReturnsAsync(MockCountries());

            var result1 = await _client.GetLookupItems(Lookup.Country);
            var result2 = await _client.GetLookupItems(Lookup.Country);

            result1.Should().BeEquivalentTo(result2);
            _mockODataClient.Verify(mock => mock.For("dfe_countries"), Times.Once);
        }

        [Fact]
        public async void GetOptionSetItems_ReturnsAll()
        {
            var query = $"EntityDefinitions(608861bc-50a4-4c5f-a02c-21fe1943e2cf)/Attributes(4e7556f1-f6c2-e811-a96b-000d3a233b72)/" +
                $"Microsoft.Dynamics.CRM.PicklistAttributeMetadata?$select=OptionSet&$expand=OptionSet";
            _mockODataClient.Setup(m => m.FindEntryAsync(query)).ReturnsAsync(MockLocations());

            var result = await _client.GetOptionSetItems(OptionSet.CandidateLocations);

            result.Select(location => location.Value).Should()
                .BeEquivalentTo(new object[] { "In the UK", "Overseas" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async void GetOptionSetItems_IsCached()
        {
            var query = $"EntityDefinitions(608861bc-50a4-4c5f-a02c-21fe1943e2cf)/Attributes(4e7556f1-f6c2-e811-a96b-000d3a233b72)/" +
                $"Microsoft.Dynamics.CRM.PicklistAttributeMetadata?$select=OptionSet&$expand=OptionSet";
            _mockODataClient.Setup(m => m.FindEntryAsync(query)).ReturnsAsync(MockLocations());

            var result1 = await _client.GetOptionSetItems(OptionSet.CandidateLocations);
            var result2 = await _client.GetOptionSetItems(OptionSet.CandidateLocations);

            result1.Should().BeEquivalentTo(result2);
            _mockODataClient.Verify(mock => mock.FindEntryAsync(query), Times.Once);
        }

        [Fact]
        public async void GetLatestPrivacyPolicy_ReturnsMostRecentActiveWebPrivacyPolicy()
        {
            _mockODataClient.Setup(m => m.For<PrivacyPolicy>(It.IsAny<string>()).Top(3)
                .Filter(p => p.IsActive && p.Type == (int) PrivacyPolicy.Types.Web)
                .OrderByDescending(p => p.CreatedAt).FindEntriesAsync()).ReturnsAsync(MockPrivacyPolicies());

            var result = await _client.GetLatestPrivacyPolicy();

            result.Text.Should().Be("Latest Active Web");
        }
        
        [Fact]
        public async void GetPrivacyPolicies_Returns3MostRecentActiveWebPrivacyPolicies()
        {
            _mockODataClient.Setup(m => m.For<PrivacyPolicy>(It.IsAny<string>()).Top(3)
                .Filter(p => p.IsActive && p.Type == (int)PrivacyPolicy.Types.Web)
                .OrderByDescending(p => p.CreatedAt).FindEntriesAsync()).ReturnsAsync(MockPrivacyPolicies());

            var result = await _client.GetPrivacyPolicies();

            result.Select(policy => policy.Text).Should().BeEquivalentTo(
                new object[] { "Latest Active Web", "Policy 2", "Policy 3" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async void GetPrivacyPolicies_IsCached()
        {
            _mockODataClient.Setup(m => m.For<PrivacyPolicy>(It.IsAny<string>()).Top(3)
                .Filter(p => p.IsActive && p.Type == (int)PrivacyPolicy.Types.Web)
                .OrderByDescending(p => p.CreatedAt).FindEntriesAsync()).ReturnsAsync(MockPrivacyPolicies());

            var result1 = await _client.GetPrivacyPolicies();
            var result2 = await _client.GetPrivacyPolicies();

            result1.Should().BeEquivalentTo(result2);
            _mockODataClient.Verify(mock => mock.For<PrivacyPolicy>(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData("john@doe.com", "New John", "Doe", "New John")]
        [InlineData("JOHN@doe.com", "New John", "Doe", "New John")]
        [InlineData("jane@doe.com", "Jane", "Doe", "Jane")]
        [InlineData("bob@doe.com", "Bob", "Doe", null)]
        public async void GetCandidate_MatchesOnNewestCandidateWithEmail(
            string email,
            string firstName,
            string lastName,
            string expectedFirstName
        )
        {
            var request = new ExistingCandidateRequest {Email = email, FirstName = firstName, LastName = lastName};
            _mockODataClient.Setup(mock => mock.For<Candidate>(It.IsAny<string>())
                .Top(20).Expand(c => c.Qualifications).Expand(c => c.PastTeachingPositions)
                .Filter(c => c.Email == request.Email).OrderByDescending(c => c.CreatedAt)
                .FindEntriesAsync()).ReturnsAsync(MockCandidates());

            var result = await _client.GetCandidate(request);
            result?.FirstName.Should().Be(expectedFirstName);
        }

        private static IEnumerable<Candidate> MockCandidates()
        {
            var candidate1 = new Candidate()
            {
                Email = "jane@doe.com",
                FirstName = "Jane",
                LastName = "Doe",
                CreatedAt = Date.Now,
            };

            var candidate2 = new Candidate()
            {
                Email = "john@doe.com",
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = Date.Now,
            };

            var candidate3 = new Candidate()
            {
                Email = "john@doe.com",
                FirstName = "Old John",
                LastName = "Doe",
                CreatedAt = DateTime.Now.AddDays(-5),
            };

            return new[] { candidate1, candidate2, candidate3 };
        }

        private static IEnumerable<PrivacyPolicy> MockPrivacyPolicies()
        {
            var policy1 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Latest Active Web" };
            var policy2 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Policy 2" };
            var policy3 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Policy 3" };

            return new List<PrivacyPolicy>() { policy1, policy2, policy3 };
        }

        private static IDictionary<string, object> MockLocations()
        {
            const string json = "[{\"Value\":222750000,\"Label\":{\"LocalizedLabels\":[{\"Label\":\"In the UK\"}]}}," +
                                "{\"Value\":222750001,\"Label\":{\"LocalizedLabels\":[{\"Label\":\"Overseas\"}]}}]";
            var locations = JArray.Parse(json);

            return new Dictionary<string, object>() { { "Options", locations.ToObject<object[]>() } };
        }

        private static List<IDictionary<string, object>> MockCountries()
        {
            var json = $"[{{\"dfe_countryid\":\"{Guid.NewGuid()}\",\"dfe_name\":\"Country 1\"}}," +
                          $"{{\"dfe_countryid\":\"{Guid.NewGuid()}\",\"dfe_name\":\"Country 2\"}}," +
                          $"{{\"dfe_countryid\":\"{Guid.NewGuid()}\",\"dfe_name\":\"Country 3\"}}]";
            var countries = JArray.Parse(json);

            return countries.ToObject<List<IDictionary<string, object>>>();
        }
    }
}
