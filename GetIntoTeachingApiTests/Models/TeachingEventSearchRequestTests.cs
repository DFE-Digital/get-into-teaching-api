using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingEventSearchRequestTests
    {
        [Theory]
        [InlineData(1, 1609.34)]
        [InlineData(-1, -1609.34)]
        [InlineData(0, 0)]
        [InlineData(87, 140012.58)]
        public void RadiusInMeters_ConvertsCorrectly(int miles, double meters)
        {
            new TeachingEventSearchRequest() {Radius = miles}.RadiusInMeters.Should().Be(meters);
        }
    }
}
