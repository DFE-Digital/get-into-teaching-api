using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidateAccessTokenChallengeTests
    {
        [Theory]
        [InlineData("  ", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("123456", true)]
        public void HasToken_ReturnsAsExpected(string token, bool expectedOutcome)
        {
            var challenge = new CandidateAccessTokenChallenge { Token = token };
            challenge.HasToken().Should().Be(expectedOutcome);
        }
    }
}
