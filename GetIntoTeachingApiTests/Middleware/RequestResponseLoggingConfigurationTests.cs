using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Middleware;
using Xunit;

namespace GetIntoTeachingApiTests.Middleware
{
    public class RequestResponseLoggingConfigurationTests
    {
        private readonly RequestResponseLoggingConfiguration _config;

        public RequestResponseLoggingConfigurationTests()
        {
            _config = new RequestResponseLoggingConfiguration();
        }

        [Theory]
        [InlineData("GET /api/callback_booking_quotas", true)]
        [InlineData("GET /api/lookup_items/item", true)]
        [InlineData("GET /api/pick_list_items/item", true)]
        [InlineData("GET /api/privacy_policies/latest", true)]
        [InlineData("GET /api/privacy_policies/item", true)]
        [InlineData("GET /api/teaching_event_buildings", true)]
        [InlineData("GET /api/teaching_events/search_indexed_by_type", true)]
        [InlineData("POST /api/teaching_events", false)]
        public void CompactLoggingPatterns_Match(string input, bool expectedOutcome)
        {
            _config.CompactLoggingPatterns.Any(regex => regex.IsMatch(input)).Should().Be(expectedOutcome);
        }
    }
}
