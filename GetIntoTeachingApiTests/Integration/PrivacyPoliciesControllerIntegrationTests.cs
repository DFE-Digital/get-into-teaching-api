using System;
using System.Net;
using System.Net.Http;
using GetIntoTeachingApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GetIntoTeachingApiTests.Integration
{
    public class PrivacyPoliciesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public PrivacyPoliciesControllerIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public void LatestPrivacyPolicy_ReturnsCorrectly()
        {
            var previous = Environment.GetEnvironmentVariable("SHARED_SECRET");
            Environment.SetEnvironmentVariable("SHARED_SECRET", "abc123");

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_client.BaseAddress}api/privacy_policies/latest"),
                Headers = {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer abc123" },
                },
            };

            var response = _client.SendAsync(httpRequestMessage).Result;

            response.EnsureSuccessStatusCode();

            Environment.SetEnvironmentVariable("SHARED_SECRET", previous);
        }
    }
}
