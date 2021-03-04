using FluentAssertions;
using GetIntoTeachingApi.Utils;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("KY11 9YU", "KY11 9YU")]
        [InlineData("KY119YU", "KY11 9YU")]
        [InlineData("ca48le", "CA4 8LE")]
        [InlineData("ca4 8LE", "CA4 8LE")]
        [InlineData("  ca4   8L E", "CA4 8LE")]
        [InlineData("invalid", null)]
        [InlineData(null, null)]
        public void AsFormattedPostcode_ReturnsCorrectly(string input, string expected)
        {
            input.AsFormattedPostcode().Should().Be(expected);
        }
    }
}
