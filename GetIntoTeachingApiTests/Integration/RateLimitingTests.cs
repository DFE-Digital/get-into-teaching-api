using FluentAssertions;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace GetIntoTeachingApiTests.Integration
{
    [Collection("Database")]
    public class RateLimitingTests : DatabaseTests
    {
        private readonly HttpClient _client;

        public RateLimitingTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            var factory = new GetIntoTeachingWebApplicationFactory(DbContext);
            _client = factory.CreateClient();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Token 1");
        }

        [Theory]
        [InlineData("/api/candidates/access_tokens", 250)]
        [InlineData("/api/mailing_list/members", 100)]
        [InlineData("/api/teaching_events/attendees", 100)]
        [InlineData("/api/teacher_training_adviser/candidates", 100)]
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
