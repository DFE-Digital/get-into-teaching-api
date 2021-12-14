using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm
{
    public class CandidateSchoolExperienceTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(CandidateSchoolExperience);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_candidateschoolexperience");

            type.GetProperty("CandidateId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_contactid" &&
                a.Type == typeof(EntityReference) && a.Reference == "contact");
            type.GetProperty("SchoolUrn").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_urn");
            type.GetProperty("DurationOfPlacementInDays").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_placementduration");
            type.GetProperty("DateOfSchoolExperience").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_dateofschoolexperience");
            type.GetProperty("Status").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "statuscode" && a.Type == typeof(OptionSetValue));
            type.GetProperty("TeachingSubjectId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_teachingsubject" && a.Type == typeof(EntityReference) &&
                     a.Reference == "dfe_teachingsubjectlist");
            type.GetProperty("Notes").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_notes");
            type.GetProperty("SchoolName").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_where");
        }
    }
}
