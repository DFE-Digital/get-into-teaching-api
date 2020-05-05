using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Validators
{
    public class CandidateAccessTokenServiceTests : IDisposable
    {
        private readonly ICandidateAccessTokenService _service;

        private string _previousTotpSecretKey;

        public CandidateAccessTokenServiceTests()
        {
            _previousTotpSecretKey = Environment.GetEnvironmentVariable("TOTP_SECRET_KEY");

            Environment.SetEnvironmentVariable("TOTP_SECRET_KEY", "secret_key");

            _service = new CandidateAccessTokenService();
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("TOTP_SECRET_KEY", _previousTotpSecretKey);
        }

        [Theory]
        [InlineData("email@address.com")]
        [InlineData("email!@address.com")]
        [InlineData("e@a.com")]
        public void GenerateToken_ReturnsAValidToken(string email)
        {
            string token = _service.GenerateToken(email);
            var challenge = new CandidateAccessTokenChallenge { Token = token, Email = email };
            _service.IsValid(challenge).Should().BeTrue();
        }

        [Fact]
        public void GenerateToken_DifferentEmailsInSameStep_ReturnDifferentTokens()
        {
            string token1 = _service.GenerateToken("email1@address.com");
            string token2 = _service.GenerateToken("email2@address.com");
            token1.Should().NotBeEquivalentTo(token2);
        }

        [Fact]
        public void GenerateToken_SameEmailInSameStep_ReturnSameToken()
        {
            string token1 = _service.GenerateToken("email@address.com");
            string token2 = _service.GenerateToken("email@address.com");
            token1.Should().BeEquivalentTo(token2);
        }

        [Theory]
        [InlineData("000000")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("abcdef")]
        public void IsValid_WithInvalidToken_ReturnsFalse(string invalidToken)
        {
            var challenge = new CandidateAccessTokenChallenge { Token = invalidToken, Email = "email@address.com" };
            _service.IsValid(challenge).Should().BeFalse();
        }


        [Fact]
        public void IsValid_WithExpiredToken_ReturnsFalse()
        {
            int secondsToOutsideOfWindow = CandidateAccessTokenService.StepInSeconds * (2 * CandidateAccessTokenService.VerificationWindow);
            var dateTimeOutsideOfWindow = DateTime.UtcNow.AddSeconds(-secondsToOutsideOfWindow);
            string token = _service.GenerateToken("email@address.com");
            var challenge = new CandidateAccessTokenChallenge { Token = token, Email = "email@address.com" };
            _service.IsValid(challenge, dateTimeOutsideOfWindow).Should().BeFalse();
        }
    }
}
