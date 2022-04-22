using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class CourseTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(Course);

            type.GetProperty("Id").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "uuid");
            type.GetProperty("Name").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "name");
        }
    }
}
