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
            entity.Attributes["dfe_preferredteachingsubject01"] = new EntityReference { Id = Guid.NewGuid() };
            entity.Attributes["dfe_preferrededucationphase01"] = new OptionSetValue { Value = 1 };
            entity.Attributes["dfe_isinuk"] = new OptionSetValue { Value = 2 };
            entity.Attributes["dfe_ittyear"] = new OptionSetValue { Value = 3 };
            entity.Attributes["emailaddress1"] = "email@address.com";
            entity.Attributes["firstname"] = "first";
            entity.Attributes["lastname"] = "last";
            entity.Attributes["birthdate"] = new DateTime(1967, 3, 10);
            entity.Attributes["address1_line1"] = "line1";
            entity.Attributes["address1_line2"] = "line2";
            entity.Attributes["address1_line3"] = "line3";
            entity.Attributes["address1_stateorprovince"] = "state";
            entity.Attributes["address1_postalcode"] = "postcode";

            var candidate = _mapper.Map<Candidate>(entity);

            candidate.Id.Should().Be(entity.Id);
            candidate.PreferredTeachingSubjectId.Should().Be(entity.GetAttributeValue<EntityReference>("dfe_preferredteachingsubject01").Id);
            candidate.PreferredEducationPhaseId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_preferrededucationphase01").Value);
            candidate.LocationId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_isinuk").Value);
            candidate.InitialTeacherTrainingYearId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_ittyear").Value);
            candidate.Email.Should().Be(entity.GetAttributeValue<string>("emailaddress1"));
            candidate.FirstName.Should().Be(entity.GetAttributeValue<string>("firstname"));
            candidate.LastName.Should().Be(entity.GetAttributeValue<string>("lastname"));
            candidate.DateOfBirth.Should().Be(entity.GetAttributeValue<DateTime>("birthdate"));
            candidate.Address.Line1.Should().Be(entity.GetAttributeValue<string>("address1_line1"));
            candidate.Address.Line2.Should().Be(entity.GetAttributeValue<string>("address1_line2"));
            candidate.Address.Line3.Should().Be(entity.GetAttributeValue<string>("address1_line3"));
            candidate.Address.State.Should().Be(entity.GetAttributeValue<string>("address1_stateorprovince"));
            candidate.Address.Postcode.Should().Be(entity.GetAttributeValue<string>("address1_postalcode"));
        }
    }
}
