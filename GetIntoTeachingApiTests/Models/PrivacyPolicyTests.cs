using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class PrivacyPolicyTests
    {
        [Fact]
        public void Constructor_WithEntity_MapsCorrectly()
        {
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity.Attributes["dfe_details"] = "text";

            var privacyPolicy = new PrivacyPolicy(entity);

            privacyPolicy.Id.Should().Be(entity.Id);
            privacyPolicy.Text.Should().Be(entity.GetAttributeValue<string>("dfe_details"));
        }
    }
}
