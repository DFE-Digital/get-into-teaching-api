using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class ApplicationChoiceTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(ApplicationChoice);

            type.GetProperty("Id").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "id");
            type.GetProperty("CreatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "created_at");
            type.GetProperty("UpdatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "updated_at");
            type.GetProperty("Status").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "status");
            type.GetProperty("Provider").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "provider");
            type.GetProperty("Course").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "course");
            type.GetProperty("Interviews").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "interviews");
        }
    }
}
