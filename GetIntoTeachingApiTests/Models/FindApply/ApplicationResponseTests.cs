using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class ApplicationResponseTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(ApplicationResponse<object>);

            type.GetProperty("Data").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "data");
            type.GetProperty("Completed").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "completed");
        }
    }
}
