using System.ComponentModel.DataAnnotations;
using FluentAssertions;
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
