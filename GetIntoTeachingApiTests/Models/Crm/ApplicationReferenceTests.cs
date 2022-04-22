using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm
{
    public class ApplicationReferenceTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(ApplicationReference);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_applyreference");
            type.Should().BeDecoratedWith<SwaggerIgnoreAttribute>();

            type.GetProperty("ApplicationFormId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_applyapplicationform"
                && a.Type == typeof(EntityReference) && a.Reference == "dfe_applyapplicationform");

            type.GetProperty("FeedbackStatusId").Should().BeDecoratedWith<EntityFieldAttribute>(
                    a => a.Name == "dfe_referencefeedbackstatus" && a.Type == typeof(OptionSetValue));

            type.GetProperty("FindApplyId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_referenceid");
            type.GetProperty("RequestedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_requestedat");
            type.GetProperty("Type").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_referencetype");
        }

        [Fact]
        public void Name_IsDerivedFromFindApplyId()
        {
            var form = new ApplicationInterview() { FindApplyId = "123" };

            form.Name.Should().Be("Application Interview 123");
        }
    }
}
