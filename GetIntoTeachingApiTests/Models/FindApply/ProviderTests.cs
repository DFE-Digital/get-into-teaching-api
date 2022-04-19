using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class ProviderTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(Provider);

            type.GetProperty("Name").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "name");
        }
    }
}
