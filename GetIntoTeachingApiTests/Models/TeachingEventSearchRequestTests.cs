using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingEventSearchRequestTests
    {
        [Fact]
        public void Loggable_IsPresent()
        {
            typeof(TeachingEventSearchRequest).Should().BeDecoratedWith<LoggableAttribute>();
        }

        [Theory]
        [InlineData(1, 1.6093)]
        [InlineData(-1, -1.6093)]
        [InlineData(0, 0)]
        [InlineData(87, 140.0125)]
        [InlineData(null, null)]
        public void RadiusInKm_ConvertsCorrectly(int? miles, double? km)
        {
            new TeachingEventSearchRequest() { Radius = miles }.RadiusInKm().Should().BeApproximately(km, 4);
        }

        [Fact]
        public void Clone_WithBlock_ClonesAndCallsBlock()
        {
            var request = new TeachingEventSearchRequest() { Radius = 10, TypeId = 123 };
            var clone = request.Clone((te) => te.Radius = 100);

            clone.Radius.Should().Be(100);
            clone.TypeId.Should().Be(request.TypeId);
        }
    }
}
