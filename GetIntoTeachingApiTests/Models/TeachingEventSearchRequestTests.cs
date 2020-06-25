using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingEventSearchRequestTests
    {
        [Theory]
        [InlineData(1, 1.6093)]
        [InlineData(-1, -1.6093)]
        [InlineData(0, 0)]
        [InlineData(87, 140.0125)]
        [InlineData(null, null)]
        public void RadiusInKm_ConvertsCorrectly(int? miles, double? km)
        {
            new TeachingEventSearchRequest() { Radius = miles }.RadiusInKm.Should().BeApproximately(km, 4);
        }
    }
}
