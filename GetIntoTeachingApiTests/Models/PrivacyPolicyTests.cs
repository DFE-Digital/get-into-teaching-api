using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class PrivacyPolicyTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(PrivacyPolicy);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_privacypolicy");

            type.GetProperty("Text").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_details");
        }

        [Fact]
        public void EntityFrameworkAttributes()
        {
            var type = typeof(PrivacyPolicy);

            type.Should().BeDecoratedWith<TableAttribute>(a => a.Name == "dfe_privacypolicies");

            type.GetProperty("Id").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_privacypolicyid");
            type.GetProperty("Text").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_details");
            type.GetProperty("Type").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_policytype");
            type.GetProperty("CreatedAt").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "createdon");
            type.GetProperty("IsActive").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_active");
        }
    }
}
