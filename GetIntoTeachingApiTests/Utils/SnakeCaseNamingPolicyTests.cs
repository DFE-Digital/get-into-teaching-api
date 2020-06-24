using FluentAssertions;
using GetIntoTeachingApi.Utils;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class SnakeCaseNamingPolicyTests
    {
        [Theory]
        [InlineData("helloWorld", "hello_world")]
        [InlineData("HelloWorld", "hello_world")]
        [InlineData("hello_world", "hello_world")]
        [InlineData("HELLOWORLD", "helloworld")]
        public void ConvertName_ConvertsCorrectly(string candidate, string expected)
        {
            SnakeCaseNamingPolicy.Instance.ConvertName(candidate).Should().Be(expected);
        }
    }
}
