using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingEventBuildingTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(TeachingEventBuilding);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "msevtmgt_building");

            type.GetProperty("AddressLine1").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_addressline1");
            type.GetProperty("AddressLine2").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_addressline2");
            type.GetProperty("AddressLine3").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_addressline3");
            type.GetProperty("AddressCity").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_city");
            type.GetProperty("AddressState").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_stateprovince");
            type.GetProperty("AddressPostcode").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_postalcode");
        }
    }
}