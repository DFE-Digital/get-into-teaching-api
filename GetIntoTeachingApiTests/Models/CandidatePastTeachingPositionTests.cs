using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidatePastTeachingPositionTests
    {
        [Fact]
        public void EntityFrameworkAttributes()
        {
            var type = typeof(CandidatePastTeachingPosition);

            type.Should().BeDecoratedWith<TableAttribute>(a => a.Name == "dfe_candidatepastteachingpositions");

            type.GetProperty("Id").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_candidatepastteachingpositionid");
            type.GetProperty("SubjectTaught").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_SubjectTaught");
            type.GetProperty("EducationPhaseId").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_educationphase");
        }
    }
}