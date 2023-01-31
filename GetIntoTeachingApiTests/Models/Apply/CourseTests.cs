using FluentAssertions;
using GetIntoTeachingApi.Models.Apply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Apply
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
