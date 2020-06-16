using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidateQualificationTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(CandidateQualification);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_candidatequalification");

            type.GetProperty("CategoryId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_category" && a.Type == typeof(OptionSetValue));
            type.GetProperty("TypeId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_type" && a.Type == typeof(OptionSetValue));
            type.GetProperty("DegreeStatusId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_degreestatus" && a.Type == typeof(OptionSetValue));
        }
    }
}
