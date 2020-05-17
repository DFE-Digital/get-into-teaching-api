using System;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidateTests
    {
        [Fact]
        public void Constructor_WithEntity_MapsCorrectly()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity["dfe_preferredteachingsubject01"] = new EntityReference { Id = Guid.NewGuid() };
            entity["dfe_preferrededucationphase01"] = new OptionSetValue { Value = 1 };
            entity["dfe_isinuk"] = new OptionSetValue { Value = 2 };
            entity["dfe_ittyear"] = new OptionSetValue { Value = 3 };
            entity["emailaddress1"] = "email@address.com";
            entity["firstname"] = "first";
            entity["lastname"] = "last";
            entity["birthdate"] = new DateTime(1967, 3, 10);
            entity["address1_line1"] = "line1";
            entity["address1_line2"] = "line2";
            entity["address1_line3"] = "line3";
            entity["address1_city"] = "city";
            entity["address1_stateorprovince"] = "state";
            entity["address1_postalcode"] = "postcode";
            entity["telephone1"] = "07564 374 624";

            var candidate = new Candidate(entity, mockService.Object);

            candidate.Id.Should().Be(entity.Id);
            candidate.PreferredTeachingSubjectId.Should().Be(entity.GetAttributeValue<EntityReference>("dfe_preferredteachingsubject01").Id);
            candidate.PreferredEducationPhaseId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_preferrededucationphase01").Value);
            candidate.LocationId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_isinuk").Value);
            candidate.InitialTeacherTrainingYearId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_ittyear").Value);
            candidate.Email.Should().Be(entity.GetAttributeValue<string>("emailaddress1"));
            candidate.FirstName.Should().Be(entity.GetAttributeValue<string>("firstname"));
            candidate.LastName.Should().Be(entity.GetAttributeValue<string>("lastname"));
            candidate.DateOfBirth.Should().Be(entity.GetAttributeValue<DateTime>("birthdate"));
            candidate.AddressLine1.Should().Be(entity.GetAttributeValue<string>("address1_line1"));
            candidate.AddressLine2.Should().Be(entity.GetAttributeValue<string>("address1_line2"));
            candidate.AddressLine3.Should().Be(entity.GetAttributeValue<string>("address1_line3"));
            candidate.AddressCity.Should().Be(entity.GetAttributeValue<string>("address1_city"));
            candidate.AddressState.Should().Be(entity.GetAttributeValue<string>("address1_stateorprovince"));
            candidate.AddressPostcode.Should().Be(entity.GetAttributeValue<string>("address1_postalcode"));
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
                AddressLine1 = "line1",
                AddressLine2 = "line2",
                AddressLine3 = "line3",
                AddressCity = "city",
                AddressState = "state",
                AddressPostcode = "postcode",
                Telephone = "07584 275 483",
                PhoneCall = new PhoneCall() { Telephone = "07435 843 274", ScheduledAt = DateTime.Now.AddDays(3) }
            };

            var entity = new Entity("contact", (Guid)candidate.Id);
            var phoneCallEntity = new Entity("phonecall") {EntityState = EntityState.Created};
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var mockContext = mockService.Object.Context("mock-connection-string");
            mockService.Setup(mock => mock.BlankExistingEntity("contact",
                entity.Id, It.IsAny<OrganizationServiceContext>())).Returns(entity);
            mockService.Setup(mock => mock.NewEntity("phonecall", It.IsAny<OrganizationServiceContext>())).Returns(phoneCallEntity);

            candidate.ToEntity(mockService.Object, mockContext);

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
            entity.GetAttributeValue<string>("address1_line1").Should().Be(candidate.AddressLine1);
            entity.GetAttributeValue<string>("address1_line2").Should().Be(candidate.AddressLine2);
            entity.GetAttributeValue<string>("address1_line3").Should().Be(candidate.AddressLine3);
            entity.GetAttributeValue<string>("address1_city").Should().Be(candidate.AddressCity);
            entity.GetAttributeValue<string>("address1_stateorprovince").Should().Be(candidate.AddressState);
            entity.GetAttributeValue<string>("address1_postalcode").Should().Be(candidate.AddressPostcode);
            entity.GetAttributeValue<string>("telephone1").Should().Be(candidate.Telephone);

            mockService.Verify(mock => mock.AddLink(entity,
                new Relationship("dfe_contact_phonecall_contactid"), It.IsAny<Entity>(), It.IsAny<OrganizationServiceContext>()));
        }
    }
}
