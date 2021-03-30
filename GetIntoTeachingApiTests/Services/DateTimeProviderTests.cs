using System;
using FluentAssertions;
using GetIntoTeachingApi.Services;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class DateTimeProviderTests
    {
        [Fact]
        public void UtcNow_ReturnsDateTimeUtcNow()
        {
            var provider = new DateTimeProvider();

            provider.UtcNow.Should().BeCloseTo(DateTime.UtcNow);
        }
    }
}
