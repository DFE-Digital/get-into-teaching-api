using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class PhoneCallTests
    {
        [Fact]
        public void EntityFrameworkAttributes()
        {
            var type = typeof(PhoneCall);

            type.Should().BeDecoratedWith<TableAttribute>(a => a.Name == "phonecall");

            type.GetProperty("Id").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "phonecallid");
            type.GetProperty("ScheduledAt").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "scheduledstart");
            type.GetProperty("Telephone").Should().BeDecoratedWith<ColumnAttribute>(a => a.Name == "phonenumber");
        }
    }
}
