using FluentAssertions;
using GetIntoTeachingApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace GetIntoTeachingApiTests.Integration
{
    public class RateLimitingTests
    {
        private readonly HttpClient _client;

        public RateLimitingTests()
        {
            var factory = new WebApplicationFactory<Startup>();
            _client = factory.CreateClient();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Token 1");
        }

        [Theory]
        [InlineData("/api/candidates/access_tokens", 60)]
        [InlineData("/api/mailing_list/members", 60)]
        [InlineData("/api/teaching_events/attendees", 60)]
        [InlineData("/api/teacher_training_adviser/candidates", 60)]
        public async void Path_ExceedingRateLimit_ReturnsStatus429TooManyRequests(string path, int limit)
        {
            HttpResponseMessage response = null;

            for (var count = 0; count < limit + 1; count++)
            {
                response = await _client.PostAsync(path, new StringContent("{}"));
            }

            response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Token 2");

            response = await _client.PostAsync(path, new StringContent("{}"));

            response.StatusCode.Should().NotBe(StatusCodes.Status429TooManyRequests);
        }
    }
}
