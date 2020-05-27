using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidatePrivacyPolicyTests
    {
        [Fact]
        public void EntityFrameworkAttributes()
        {
            var type = typeof(CandidatePrivacyPolicy);

            type.Should().BeDecoratedWith<TableAttribute>(a => a.Name == "dfe_candidateprivacypolicy");

            type.GetProperty("Id").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_candidateprivacypolicyid");
            type.GetProperty("AcceptedPolicy").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "dfe_PrivacyPolicyNumber");
        }
    }
}
