using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm
{
    public class CandidateQualificationTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(CandidateQualification);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_candidatequalification");
            type.Should().BeDecoratedWith<SwaggerIgnoreAttribute>();

            type.GetProperty("CandidateId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_contactid" &&
                a.Type == typeof(EntityReference) && a.Reference == "contact");

            type.GetProperty("UkDegreeGradeId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_ukdegreegrade" && a.Type == typeof(OptionSetValue));
            type.GetProperty("TypeId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_type" && a.Type == typeof(OptionSetValue));
            type.GetProperty("DegreeStatusId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_degreestatus" && a.Type == typeof(OptionSetValue));

            type.GetProperty("DegreeSubject").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_subject");
            type.GetProperty("CreatedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "createdon");
        }
    }
}
