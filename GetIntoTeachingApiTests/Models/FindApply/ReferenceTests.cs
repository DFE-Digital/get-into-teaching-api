using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class ReferenceTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(Reference);

            type.GetProperty("Id").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "id");
            type.GetProperty("RequestedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "requested_at");
            type.GetProperty("FeedbackStatus").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "feedback_status");
            type.GetProperty("RefereeType").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "referee_type");
        }
    }
}
