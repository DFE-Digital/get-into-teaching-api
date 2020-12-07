using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GetIntoTeachingApiTests.Attributes
{
    public class PrivateShortTermResponseCacheAttributeTests
    {
        [Fact]
        public void Constructor_CorrectlySetsCacheControl()
        {
            var cache = new PrivateShortTermResponseCacheAttribute();

            cache.Location.Should().Be(ResponseCacheLocation.Client);
            cache.Duration.Should().Be(300);
        }
    }
}
