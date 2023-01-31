using FluentAssertions;
using GetIntoTeachingApi.Models.Apply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Apply
{
    public class ResponseTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(Response<object>);

            type.GetProperty("Data").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "data");
        }
    }
}
