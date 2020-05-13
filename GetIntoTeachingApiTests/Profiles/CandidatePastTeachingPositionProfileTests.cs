using AutoMapper;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApiTests.Utils;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Profiles
{
    public class CandidatePastTeachingPositionProfileTests
    {
        private readonly Mapper _mapper;

        public CandidatePastTeachingPositionProfileTests()
        {
            _mapper = MapperHelpers.CreateMapper();
        }

        [Fact]
        public void CandidatePastTeachingPositionProfile_MapsCorrectly()
        {
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity.Attributes["dfe_subjecttaught"] = new EntityReference { Id = Guid.NewGuid() };
            entity.Attributes["dfe_educationphase"] = new OptionSetValue { Value = 1 };

            var candidate = _mapper.Map<CandidatePastTeachingPosition>(entity);

            candidate.Id.Should().Be(entity.Id);
            candidate.SubjectTaughtId.Should().Be(entity.GetAttributeValue<EntityReference>("dfe_subjecttaught").Id);
            candidate.EducationPhaseId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_educationphase").Value);
        }
    }
}