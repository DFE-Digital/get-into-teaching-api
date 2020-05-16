using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
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
            privacyPolicy.ToEntity(entity);

            entity.GetAttributeValue<EntityReference>("dfe_privacypolicynumber").Id.Should()
                .Be(privacyPolicy.AcceptedPolicyId);
            entity.GetAttributeValue<EntityReference>("dfe_privacypolicynumber").LogicalName.Should()
                .Be("dfe_privacypolicy");
        }
    }
}
