using AutoMapper;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApiTests.Utils;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Profiles
{
    public class CandidateQualificationProfileTests
    {
        private readonly Mapper _mapper;

        public CandidateQualificationProfileTests()
        {
            _mapper = MapperHelpers.CreateMapper();
        }

        [Fact]
        public void CandidateQualificationProfile_MapsCorrectly()
        {
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity.Attributes["dfe_category"] = new OptionSetValue { Value = 1 };
            entity.Attributes["dfe_type"] = new OptionSetValue { Value = 2 };
            entity.Attributes["dfe_degreestatus"] = new OptionSetValue { Value = 3 };

            var candidate = _mapper.Map<CandidateQualification>(entity);

            candidate.Id.Should().Be(entity.Id);
            candidate.CategoryId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_category").Value);
            candidate.TypeId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_type").Value);
            candidate.DegreeStatusId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_degreestatus").Value);
        }
    }
}
