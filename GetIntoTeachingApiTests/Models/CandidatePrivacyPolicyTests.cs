using System;
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

            type.GetProperty("ConsentReceivedById").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_consentreceivedby");
            type.GetProperty("MeanOfConsentId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_meanofconsent");

            type.GetProperty("Description").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_name");
            type.GetProperty("AcceptedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_timeofconsent");
        }

        [Fact]
        public void ConsentReceivedById_DefaultValue_IsCorrect()
        {
            new CandidatePrivacyPolicy().ConsentReceivedById.Should().Be(CandidatePrivacyPolicy.Consent);
        }

        [Fact]
        public void MeanOfConsentId_DefaultValue_IsCorrect()
        {
            new CandidatePrivacyPolicy().MeanOfConsentId.Should().Be(CandidatePrivacyPolicy.Consent);
        }

        [Fact]
        public void Description_DefaultValue_IsCorrect()
        {
            new CandidatePrivacyPolicy().Description.Should().Be("Online consent as part of web registration");
        }

        [Fact]
        public void AcceptedAt_DefaultValue_IsNow()
        {
            new CandidatePrivacyPolicy().AcceptedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }
    }
}
