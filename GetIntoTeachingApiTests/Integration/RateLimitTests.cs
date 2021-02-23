using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GetIntoTeachingApi;
using GetIntoTeachingApiTests.Helpers;
using Xunit;

namespace GetIntoTeachingApiTests.Integration
{
    [Collection("Database")]
    public class RateLimitTests : IClassFixture<GitWebApplicationFactory<Startup>>
    {
        private readonly GitWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _httpClient;
        private readonly StringContent _emptyBody;

        public RateLimitTests(GitWebApplicationFactory<Startup> factory)
        {
            Environment.SetEnvironmentVariable($"GIT_API_KEY", "git-secret");
            Environment.SetEnvironmentVariable($"ADMIN_API_KEY", "admin-secret");
            Environment.SetEnvironmentVariable($"TTA_API_KEY", "tta-secret");

            _factory = factory;

            _httpClient = _factory.CreateClient();
            _emptyBody = new StringContent("{}", Encoding.UTF8, "application/json");
        }

        [Theory]
        [InlineData("/api/candidates/access_tokens", "GIT", 500)]
        [InlineData("/api/mailing_list/members", "GIT", 250)]
        [InlineData("/api/teaching_events/attendees", "GIT", 250)]
        [InlineData("/api/candidates/access_tokens", "TTA", 500)]
        [InlineData("/api/teacher_training_adviser/candidates", "TTA", 250)]
        public async Task Post_Endpoint_AppliesRateLimit(string path, string client, int limit)
        {
            var apiKey = $"{client.ToLower()}-secret";

            _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

            HttpResponseMessage response = null;

            for (var i = 0; i < limit; i++)
            {
                response = await _httpClient.PostAsync(path, _emptyBody);
            }

            response.StatusCode.Should().NotBe(HttpStatusCode.TooManyRequests);

            response = await _httpClient.PostAsync(path, _emptyBody);

            response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        }
    }
}
