using System;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class PrivacyPolicyTests
    {
        [Fact]
        public void Constructor_WithEntity_MapsCorrectly()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity["dfe_details"] = "text";

            var privacyPolicy = new PrivacyPolicy(entity, mockService.Object);

            privacyPolicy.Id.Should().Be(entity.Id);
            privacyPolicy.Text.Should().Be(entity.GetAttributeValue<string>("dfe_details"));
        }
    }
}
