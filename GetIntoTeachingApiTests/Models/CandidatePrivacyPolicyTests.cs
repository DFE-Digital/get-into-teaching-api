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
    public class CandidatePrivacyPolicyTests
    {
        [Fact]
        public void ToEntity_ReverseMapsCorrectly()
        {
            var privacyPolicy = new CandidatePrivacyPolicy()
            {
                AcceptedPolicyId = Guid.NewGuid(),
            };

            var entity = new Entity("dfe_candidateprivacypolicy");
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var mockContext = mockService.Object.Context("mock-connection-string");
            mockService.Setup(mock => mock.NewEntity("dfe_candidateprivacypolicy", 
                It.IsAny<OrganizationServiceContext>())).Returns(entity);

            privacyPolicy.ToEntity(mockService.Object, mockContext);


            entity.GetAttributeValue<EntityReference>("dfe_privacypolicynumber").Id.Should()
                .Be(privacyPolicy.AcceptedPolicyId);
            entity.GetAttributeValue<EntityReference>("dfe_privacypolicynumber").LogicalName.Should()
                .Be("dfe_privacypolicy");
        }
    }
}
