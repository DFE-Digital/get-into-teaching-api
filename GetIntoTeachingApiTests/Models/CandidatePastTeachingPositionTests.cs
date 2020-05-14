using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidatePastTeachingPositionTests
    {
        [Fact]
        public void Constructor_WithEntity_MapsCorrectly()
        {
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity.Attributes["dfe_subjecttaught"] = new EntityReference("dfe_teachingsubjectlist", Guid.NewGuid());
            entity.Attributes["dfe_educationphase"] = new OptionSetValue { Value = 1 };

            var teachingPosition = new CandidatePastTeachingPosition(entity);

            teachingPosition.Id.Should().Be(entity.Id);
            teachingPosition.SubjectTaughtId.Should().Be(entity.GetAttributeValue<EntityReference>("dfe_subjecttaught").Id);
            teachingPosition.EducationPhaseId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_educationphase").Value);
        }

        [Fact]
        public void ToEntity_ReverseMapsCorrectly()
        {
            var teachingPosition = new CandidatePastTeachingPosition()
            {
                Id = Guid.NewGuid(),
                SubjectTaughtId = Guid.NewGuid(),
                EducationPhaseId = 1,
            };

            var entity = teachingPosition.ToEntity();

            entity.LogicalName.Should().Be("dfe_candidatepastteachingposition");
            entity.Id.Should().Be((Guid)teachingPosition.Id);
            entity.GetAttributeValue<EntityReference>("dfe_subjecttaught").Id.Should()
                .Be((Guid)teachingPosition.SubjectTaughtId);
            entity.GetAttributeValue<EntityReference>("dfe_subjecttaught").LogicalName.Should()
                .Be("dfe_teachingsubjectlist");
            entity.GetAttributeValue<OptionSetValue>("dfe_educationphase").Value.Should()
                .Be(teachingPosition.EducationPhaseId);
        }
    }
}