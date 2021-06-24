using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
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
