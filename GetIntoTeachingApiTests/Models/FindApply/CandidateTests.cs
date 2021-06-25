using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class CandidateTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(Candidate);

            type.GetProperty("Id").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "id");
            type.GetProperty("Attributes").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "attributes");
        }
    }
}
