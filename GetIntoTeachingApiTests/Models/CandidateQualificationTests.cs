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
    public class CandidateQualificationTests
    {
        [Fact]
        public void Constructor_WithEntity_MapsCorrectly()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity["dfe_category"] = new OptionSetValue { Value = 1 };
            entity["dfe_type"] = new OptionSetValue { Value = 2 };
            entity["dfe_degreestatus"] = new OptionSetValue { Value = 3 };

            var qualification = new CandidateQualification(entity, mockService.Object);

            qualification.Id.Should().Be(entity.Id);
            qualification.CategoryId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_category").Value);
            qualification.TypeId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_type").Value);
            qualification.DegreeStatusId.Should().Be(entity.GetAttributeValue<OptionSetValue>("dfe_degreestatus").Value);
        }
        
        [Fact]
        public void ToEntity_ReverseMapsCorrectly()
        {
            var qualification = new CandidateQualification()
            {
                Id = Guid.NewGuid(),
                CategoryId = 1,
                TypeId = 2,
                DegreeStatusId = 3,
            };

            var entity = new Entity("dfe_candidatequalification", (Guid)qualification.Id);
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var mockContext = mockService.Object.Context("mock-connection-string");
            mockService.Setup(mock => mock.BlankExistingEntity("dfe_candidatequalification",
                entity.Id, It.IsAny<OrganizationServiceContext>())).Returns(entity);

            qualification.ToEntity(mockService.Object, mockContext);

            entity.GetAttributeValue<OptionSetValue>("dfe_category").Value.Should().Be(qualification.CategoryId);
            entity.GetAttributeValue<OptionSetValue>("dfe_type").Value.Should().Be(qualification.TypeId);
            entity.GetAttributeValue<OptionSetValue>("dfe_degreestatus").Value.Should().Be(qualification.DegreeStatusId);
        }
    }
}
