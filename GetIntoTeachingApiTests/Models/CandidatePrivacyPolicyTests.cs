using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidatePrivacyPolicyTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(CandidatePrivacyPolicy);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_candidateprivacypolicy");

            type.GetProperty("AcceptedPolicyId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_privacypolicynumber" && a.Type == typeof(EntityReference) && a.Reference == "dfe_privacypolicy");
        }
    }
}
