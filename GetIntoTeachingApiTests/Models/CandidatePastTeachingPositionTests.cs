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
            entity["dfe_subjecttaught"] = new EntityReference("dfe_teachingsubjectlist", Guid.NewGuid());
            entity["dfe_educationphase"] = new OptionSetValue { Value = 1 };

            var position = new CandidatePastTeachingPosition(entity);

            position.Id.Should().Be(entity.Id);
            position.SubjectTaughtId.Should().Be(entity.GetAttributeValue<EntityReference>("dfe_subjecttaught").Id);
            position.EducationPhaseId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_educationphase").Value);
        }

        [Fact]
        public void ToEntity_ReverseMapsCorrectly()
        {
            var position = new CandidatePastTeachingPosition()
            {
                Id = Guid.NewGuid(),
                SubjectTaughtId = Guid.NewGuid(),
                EducationPhaseId = 1,
            };

            var entity = new Entity("dfe_candidatepastteachingposition", (Guid) position.Id);
            position.ToEntity(entity);

            entity.GetAttributeValue<EntityReference>("dfe_subjecttaught").Id.Should().Be((Guid)position.SubjectTaughtId);
            entity.GetAttributeValue<EntityReference>("dfe_subjecttaught").LogicalName.Should().Be("dfe_teachingsubjectlist");
            entity.GetAttributeValue<OptionSetValue>("dfe_educationphase").Value.Should().Be(position.EducationPhaseId);
        }
    }
}