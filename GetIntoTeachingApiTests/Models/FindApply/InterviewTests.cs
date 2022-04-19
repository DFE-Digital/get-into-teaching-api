using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class InterviewTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(Interview);

            type.GetProperty("Id").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "id");
            type.GetProperty("DateAndTime").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "date_and_time");
            type.GetProperty("CreatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "created_at");
            type.GetProperty("UpdatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "updated_at");
            type.GetProperty("CancelledAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "cancelled_at");
        }
    }
}
