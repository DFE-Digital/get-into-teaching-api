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
            type.GetProperty("CreatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "created_at");
            type.GetProperty("UpdatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "updated_at");
            type.GetProperty("ApplicationForms").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "application_forms");
        }
    }
}
