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

        [Theory]
        [InlineData("001234567", "1234567")]
        [InlineData("0001234567", "01234567")]
        [InlineData("1234567", "1234567")]
        [InlineData(null, null)]
        public void StripExitCode_ReturnsCorrectly(string input, string expected)
        {
            input.StripExitCode().Should().Be(expected);
        }

        [Theory]
        [InlineData("(65).234.543.435", "0065234543435", true)]
        [InlineData("+818495394", "00818495394", true)]
        [InlineData("+(81) 849 5394", "00818495394", true)]
        [InlineData("+44756483443", "0756483443", true)]
        [InlineData("+440756483443", "0756483443", true)]
        [InlineData("07584758473", "07584758473", false)]
        public void AsFormattedTelephone_IsSanitizedCorrectly(string input, string expected, bool international)
        {
            input.AsFormattedTelephone(international).Should().Be(expected);
        }

        [Theory]
        [InlineData("one_two_three", "OneTwoThree")]
        [InlineData("ONE_TWO_THREE", "OneTwoThree")]
        [InlineData("One Two Three", "OneTwoThree")]
        public void ToPascalCase_IsConvertedCorrectly(string input, string expected)
        {
            input.ToPascalCase().Should().Be(expected);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("  ", false)]
        [InlineData("true", true)]
        [InlineData("True", true)]
        [InlineData("TRUE", true)]
        [InlineData("1", true)]
        [InlineData("on", true)]
        [InlineData("ON", true)]
        [InlineData(" on ", true)]
        [InlineData("false", false)]
        [InlineData("f", false)]
        [InlineData("off", false)]
        [InlineData("Banana", false)]
        public void ToBool_ReturnsCorrectly(string input, bool expected)
        {
            input.ToBool().Should().Be(expected);
        }
    }
}
