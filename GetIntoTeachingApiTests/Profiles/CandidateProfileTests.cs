using AutoMapper;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApiTests.Utils;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Profiles
{
    public class CandidateProfileTests
    {
        private readonly Mapper _mapper;

        public CandidateProfileTests()
        {
            _mapper = MapperHelpers.CreateMapper();
        }

        [Fact]
        public void CandidateProfile_MapsCorrectly()
        {
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity.Attributes["emailaddress1"] = "email@address.com";
            entity.Attributes["firstname"] = "first";
            entity.Attributes["lastname"] = "last";
            entity.Attributes["birthdate"] = new DateTime(1967, 3, 10);

            var candidate = _mapper.Map<Candidate>(entity);

            candidate.Id.Should().Be(entity.Id);
            candidate.Email.Should().Be(entity.GetAttributeValue<string>("emailaddress1"));
            candidate.FirstName.Should().Be(entity.GetAttributeValue<string>("firstname"));
            candidate.LastName.Should().Be(entity.GetAttributeValue<string>("lastname"));
            candidate.DateOfBirth.Should().Be(entity.GetAttributeValue<DateTime>("birthdate"));
        }
    }
}
