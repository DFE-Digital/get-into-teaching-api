using FluentAssertions;
using GetIntoTeachingApi.Services;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class LocationServiceTests
    {
        private readonly ILocationService _service;

        public LocationServiceTests()
        {
            _service = new LocationService("./Fixtures/ukpostcodes.csv");
        }

        [Theory]
        [InlineData("KY11 9YU")]
        [InlineData("ky11 9yu")]
        [InlineData("ky119yu")]
        [InlineData("k y 119 YU")]
        [InlineData("CA4 8LE")]
        public void IsValid_WithValidPostcode_ReturnsTrue(string postcode)
        {
            _service.IsValid(postcode).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("KY11 9ZZ")]
        [InlineData("KY11 9HFF")]
        [InlineData("Non-Geographic")]
        public void IsValid_WithInvalidPostcode_ReturnsFalse(string postcode)
        {
            _service.IsValid(postcode).Should().BeFalse();
        }

        [Fact]
        public void DistanceBetween_ReturnsDistanceInMiles()
        {
            var distance = _service.DistanceBetween("KY11 9YU", "CA4 8LE");
            distance.Should().BeApproximately(81, 0.5);
        }
    }
}
