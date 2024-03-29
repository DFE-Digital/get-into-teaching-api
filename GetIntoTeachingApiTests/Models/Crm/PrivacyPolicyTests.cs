﻿using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm
{
    public class PrivacyPolicyTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(PrivacyPolicy);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_privacypolicy");

            type.GetProperty("Text").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_details");
            type.GetProperty("CreatedAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "createdon");
        }
    }
}
