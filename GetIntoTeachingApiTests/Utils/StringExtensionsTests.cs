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
        [InlineData("M12WD", "M1 2WD")]
        [InlineData("ca4 8LE", "CA4 8LE")]
        [InlineData("  ca4   8L E", "CA4 8LE")]
        [InlineData("invalid", null)]
        [InlineData(null, null)]
        public void AsFormattedPostcode_ReturnsCorrectly(string input, string expected)
        {
            input.AsFormattedPostcode().Should().Be(expected);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("  ", null)]
        [InlineData("test", "test")]
        [InlineData(" test ", " test ")]
        public void NullIfEmptyOrWhitespace_ReturnsConrrectly(string input, string expected)
        {
            input.NullIfEmptyOrWhitespace().Should().Be(expected);
        }
    }
}
