using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class CandidateAttributesTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(CandidateAttributes);

            type.GetProperty("Email").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "email_address");
        }
    }
}
