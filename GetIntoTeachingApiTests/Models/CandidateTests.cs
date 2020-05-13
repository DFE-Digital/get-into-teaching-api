using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidateTests
    {
        [Fact]
        public void Constructor_WithEntity_MapsCorrectly()
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
            entity.Attributes["telephone1"] = "07564 374 624";

            var candidate = new Candidate(entity);

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
            candidate.Telephone.Should().Be(entity.GetAttributeValue<string>("telephone1"));
        }

        [Fact]
        public void ToEntity_ReverseMapsCorrectly()
        {
            var candidate = new Candidate()
            {
                Id = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                PreferredEducationPhaseId = 1,
                LocationId = 2,
                InitialTeacherTrainingYearId = 3,
                Email = "email@address.com",
                FirstName = "first",
                LastName = "last",
                DateOfBirth = new DateTime(1967, 3, 10),
                Address = new Address()
                {
                    Line1 = "line1",
                    Line2 = "line2",
                    Line3 = "line3",
                    City = "city",
                    State = "state",
                    Postcode = "postcode",
                },
                Telephone = "07584 275 483"
            };

            var entity = candidate.ToEntity();

            entity.LogicalName.Should().Be("contact");
            entity.Id.Should().Be((Guid)candidate.Id);
            entity.GetAttributeValue<EntityReference>("dfe_preferredteachingsubject01").Id.Should()
                .Be((Guid)candidate.PreferredTeachingSubjectId);
            entity.GetAttributeValue<EntityReference>("dfe_preferredteachingsubject01").LogicalName.Should()
                .Be("dfe_teachingsubjectlist");
            entity.GetAttributeValue<OptionSetValue>("dfe_preferrededucationphase01").Value.Should()
                .Be(candidate.PreferredEducationPhaseId);
            entity.GetAttributeValue<OptionSetValue>("dfe_isinuk").Value.Should()
                .Be(candidate.LocationId);
            entity.GetAttributeValue<OptionSetValue>("dfe_ittyear").Value.Should()
                .Be(candidate.InitialTeacherTrainingYearId);
            entity.GetAttributeValue<string>("firstname").Should().Be(candidate.FirstName);
            entity.GetAttributeValue<string>("lastname").Should().Be(candidate.LastName);
            entity.GetAttributeValue<DateTime>("birthdate").Should().Be(new DateTime(1967, 3, 10));
            entity.GetAttributeValue<string>("address1_line1").Should().Be(candidate.Address.Line1);
            entity.GetAttributeValue<string>("address1_line2").Should().Be(candidate.Address.Line2);
            entity.GetAttributeValue<string>("address1_line3").Should().Be(candidate.Address.Line3);
            entity.GetAttributeValue<string>("address1_city").Should().Be(candidate.Address.City);
            entity.GetAttributeValue<string>("address1_stateorprovince").Should().Be(candidate.Address.State);
            entity.GetAttributeValue<string>("address1_postalcode").Should().Be(candidate.Address.Postcode);
            entity.GetAttributeValue<string>("telephone1").Should().Be(candidate.Telephone);
        }
    }
}
