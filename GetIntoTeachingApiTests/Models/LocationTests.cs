using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using GeoCoordinatePortable;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class LocationTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(Location);

            type.GetProperty("Postcode").Should().BeDecoratedWith<KeyAttribute>();
        }

        [Fact]
        public void Coordinate_ForNonGeographic_ReturnsNull()
        {
            new Location() { Latitude = null, Longitude = null }.Coordinate.Should().BeNull();
        }

        [Fact]
        public void Coordinate_ForGeographic_ReturnsCorrectly()
        {
            var coordinate = new GeoCoordinate(1.234, 5.678);
            new Location() { Latitude = 1.234, Longitude = 5.678 }.Coordinate.Should().BeEquivalentTo(coordinate);
        }

        [Theory]
        [InlineData(null, null, true)]
        [InlineData(null, 1.234, true)]
        [InlineData(1.234, null, true)]
        [InlineData(1.234, 5.678, false)]
        public void IsGeographic_ReturnsCorrectly(double? lat, double? lng, bool expected)
        {
            new Location() {Latitude = lat, Longitude = lng}.IsNonGeographic().Should().Be(expected);
        }
    }
}
