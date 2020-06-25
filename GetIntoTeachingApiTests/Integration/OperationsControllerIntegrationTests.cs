using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using FluentAssertions;
using GetIntoTeachingApi;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using WireMock;
using Xunit;

namespace GetIntoTeachingApiTests.Integration
{
    public class OperationsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public OperationsControllerIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async void GenerateMappingInfo_ReturnsCorrectly()
        {
            var httpResponse = await _client.GetAsync("/api/operations/generate_mapping_info");

            httpResponse.EnsureSuccessStatusCode();

            var json = await httpResponse.Content.ReadAsStringAsync();
            var mappingInfo = JsonSerializer.Deserialize<IEnumerable<MappingInfo>>(json);

            mappingInfo.Count().Should().Be(10);
        }
    }
}
