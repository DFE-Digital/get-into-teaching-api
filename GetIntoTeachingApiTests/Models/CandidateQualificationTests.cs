using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidateQualificationTests
    {
        [Fact]
        public void EntityFrameworkAttributes()
        {
            var type = typeof(CandidateQualification);

            type.Should().BeDecoratedWith<TableAttribute>(a => a.Name == "dfe_candidatequalifications");

            type.GetProperty("Id").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_candidatequalificationid");
            type.GetProperty("CategoryId").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_category");
            type.GetProperty("TypeId").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_type");
            type.GetProperty("DegreeStatusId").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_degreestatus");
        }
    }
}
