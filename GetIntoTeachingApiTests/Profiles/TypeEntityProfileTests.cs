using AutoMapper;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApiTests.Utils;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Profiles
{
    public class TypeEntityProfileTests
    {
        private readonly Mapper _mapper;

        public TypeEntityProfileTests()
        {
            _mapper = MapperHelpers.CreateMapper();
        }

        [Fact]
        public void TypeEntityProfile_MapsCorrectly()
        {
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity.Attributes["dfe_name"] = "name";

            var typeEntity = _mapper.Map<TypeEntity>(entity);

            typeEntity.Id.Should().Be(entity.Id);
            (typeEntity.Value as string).Should().Be(entity.GetAttributeValue<string>("dfe_name"));
        }
    }
}
