using System.Linq;
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
                a => a.Name == "dfe_applyphase" && a.Type == typeof(OptionSetValue));
            type.GetProperty("StatusId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_applystatus" && a.Type == typeof(OptionSetValue));
            type.GetProperty("RecruitmentCycleYearId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_recruitmentyear" && a.Type == typeof(OptionSetValue));

            type.GetProperty("FindApplyId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_applicationformid");
            type.GetProperty("CreatedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_createdon");
            type.GetProperty("UpdatedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_modifiedon");
            type.GetProperty("SubmittedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_submittedatdate");
            type.GetProperty("Name").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_name");
            type.GetProperty("QualificationsCompleted").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_qualificationscompleted" && a.Features.Contains("APPLY_API_V1_2"));
            type.GetProperty("ReferencesCompleted").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_referencescompleted" && a.Features.Contains("APPLY_API_V1_2"));
            type.GetProperty("ApplicationChoicesCompleted").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_applicationchoicescompleted" && a.Features.Contains("APPLY_API_V1_2"));
            type.GetProperty("PersonalStatementCompleted").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_personalstatementcompleted" && a.Features.Contains("APPLY_API_V1_2"));

            type.GetProperty("Choices").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "dfe_applyapplicationform_dfe_applyapplicationchoice_applyapplicationform" &&
                     a.Type == typeof(ApplicationChoice));
            type.GetProperty("References").Should().BeDecoratedWith<EntityRelationshipAttribute>(
                a => a.Name == "dfe_applyapplicationform_dfe_applyreference_applyapplicationform" &&
                        a.Type == typeof(ApplicationReference));
        }

        [Fact]
        public void Name_IsDerivedFromFindApplyId()
        {
            var form = new ApplicationForm() { FindApplyId = "123" };

            form.Name.Should().Be("Application Form 123");
        }
    }
}
