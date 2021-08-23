using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm
{
    public class ApplicationFormTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(ApplicationForm);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_applyapplicationform");
            type.Should().BeDecoratedWith<SwaggerIgnoreAttribute>();

            type.GetProperty("CandidateId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_contact"
                && a.Type == typeof(EntityReference) && a.Reference == "contact");

            type.GetProperty("PhaseId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_candidateapplyphase" && a.Type == typeof(OptionSetValue));

            type.GetProperty("FindApplyId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_applicationformid");
            type.GetProperty("CreatedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_createdon");
            type.GetProperty("UpdatedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_modifiedon");
            type.GetProperty("Name").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_name");
        }

        [Fact]
        public void Name_IsDerivedFromFindApplyId()
        {
            var form = new ApplicationForm() { FindApplyId = "123" };

            form.Name.Should().Be("Application Form 123");
        }
    }
}