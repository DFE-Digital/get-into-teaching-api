using FluentAssertions;
using GetIntoTeachingApi.Models.FindApply;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Models.FindApply
{
    public class ApplicationFormTests
    {
        [Fact]
        public void JsonAttributes()
        {
            var type = typeof(ApplicationForm);

            type.GetProperty("Id").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "id");
            type.GetProperty("CreatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "created_at");
            type.GetProperty("UpdatedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "updated_at");
            type.GetProperty("SubmittedAt").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "submitted_at");
            type.GetProperty("ApplicationStatus").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "application_status");
            type.GetProperty("ApplicationPhase").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "application_phase");
            type.GetProperty("RecruitmentCycleYear").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "recruitment_cycle_year");
        }
    }
}
