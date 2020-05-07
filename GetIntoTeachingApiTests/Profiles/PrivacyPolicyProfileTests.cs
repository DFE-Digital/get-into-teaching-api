using AutoMapper;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApiTests.Utils;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Profiles
{
    public class PrivacyPolicyProfileTests
    {
        private readonly Mapper _mapper;

        public PrivacyPolicyProfileTests()
        {
            _mapper = MapperHelpers.CreateMapper();
        }

        [Fact]
        public void PrivacyPolicyProfile_MapsCorrectly()
        {
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity.Attributes["dfe_details"] = "text";

            var privacyPolicy = _mapper.Map<PrivacyPolicy>(entity);

            privacyPolicy.Id.Should().Be(entity.Id);
            privacyPolicy.Text.Should().Be(entity.GetAttributeValue<string>("dfe_details"));
        }
    }
}
