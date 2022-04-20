using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm
{
    public class ApplicationChoiceTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(ApplicationChoice);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_applyapplicationchoice");
            type.Should().BeDecoratedWith<SwaggerIgnoreAttribute>();

            type.GetProperty("ApplicationFormId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_applyapplicationform"
                && a.Type == typeof(EntityReference) && a.Reference == "dfe_applyapplicationform");

            type.GetProperty("StatusId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_applicationchoicestatus" && a.Type == typeof(OptionSetValue));

            type.GetProperty("FindApplyId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_applicationchoiceid");
            type.GetProperty("CreatedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_createdon");
            type.GetProperty("UpdatedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_modifiedon");
            type.GetProperty("CourseName").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_applicationchoicecoursename");
            type.GetProperty("CourseId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_applicationchoicecourseuuid");
            type.GetProperty("Provider").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_applicationchoiceprovider");

            type.GetProperty("Interviews").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "dfe_applyapplicationform_dfe_applyapplicationinterview_applyapplicationform" &&
                     a.Type == typeof(ApplicationInterview));
        }

        [Fact]
        public void Name_IsDerivedFromFindApplyId()
        {
            var form = new ApplicationChoice() { FindApplyId = "123" };

            form.Name.Should().Be("Application Choice 123");
        }
    }
}
