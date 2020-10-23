using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models;
using NetTopologySuite;
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
        public void Constructor_WithPostcode()
        {
            var location = new Location("KY11 9YU");

            location.Postcode.Should().Be("ky119yu");
        }

        [Fact]
        public void Constructor_WithLatitudeLongitude()
        {
            var location = new Location("KY11 9YU", 1, 2);

            location.Postcode.Should().Be("ky119yu");

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: DbConfiguration.Wgs84Srid);
            var expectedCoordinate = geometryFactory.CreatePoint(new NetTopologySuite.Geometries.Coordinate(2, 1));
            location.Coordinate.Should().Be(expectedCoordinate);
        }

        [Fact]
        public void Constructor_WithCoordinate()
        {
            var coordinate = new NetTopologySuite.Geometries.Point(new NetTopologySuite.Geometries.Coordinate(1, 2));
            var location = new Location("KY11 9YU", coordinate);

            location.Postcode.Should().Be("ky119yu");
            location.Coordinate.Should().Be(coordinate);
        }

        [Theory]
        [InlineData("KY2 5FS", "ky25fs")]
        [InlineData("ca38kf", "ca38kf")]
        [InlineData(" ky1   5h f", "ky15hf")]
        public void SanitizePostcode(string postcode, string expected)
        {
            Location.SanitizePostcode(postcode).Should().Be(expected);
        }
    }
}
