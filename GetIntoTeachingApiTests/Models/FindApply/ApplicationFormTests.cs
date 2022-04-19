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
            type.GetProperty("ApplicationChoicesCompleted").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "application_choices.completed");
            type.GetProperty("ApplicationChoices").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "application_choices.data");
            type.GetProperty("ReferencesCompleted").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "references.completed");
            type.GetProperty("References").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "references.data");
            type.GetProperty("QualificationsCompleted").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "qualifications.completed");
            type.GetProperty("PersonalStatementCompleted").Should()
                .BeDecoratedWith<JsonPropertyAttribute>(a => a.PropertyName == "personal_statement.completed");
        }
    }
}
