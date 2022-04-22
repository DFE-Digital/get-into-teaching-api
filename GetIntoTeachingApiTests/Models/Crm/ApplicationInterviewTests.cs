using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm
{
    public class ApplicationInterviewTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(ApplicationInterview);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_applyinterview");
            type.Should().BeDecoratedWith<SwaggerIgnoreAttribute>();

            type.GetProperty("ApplicationChoiceId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_applyapplicationchoice"
                && a.Type == typeof(EntityReference) && a.Reference == "dfe_applyapplicationchoice");

            type.GetProperty("FindApplyId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_interviewid");
            type.GetProperty("CreatedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_createdon");
            type.GetProperty("UpdatedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_modifiedon");
            type.GetProperty("ScheduledAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_interviewscheduledat");
            type.GetProperty("CancelledAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_interviewcancelledat");
        }

        [Fact]
        public void Name_IsDerivedFromFindApplyId()
        {
            var form = new ApplicationInterview() { FindApplyId = "123" };

            form.Name.Should().Be("Application Interview 123");
        }
    }
}
