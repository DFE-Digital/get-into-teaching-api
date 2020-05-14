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
        private readonly string _previousTotpSecretKey;

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
        [InlineData("email@address.com", "John", "Doe")]
        [InlineData("email!@address.com", "Jane", "Smith" )]
        [InlineData("e@a.com", "B", "C")]
        public void GenerateToken_ReturnsAValidToken(string email, string firstName, string lastName)
        {
            var request = new ExistingCandidateRequest { Email = email, FirstName = firstName, LastName = lastName };
            var token = _service.GenerateToken(request);

            _service.IsValid(token, request).Should().BeTrue();
        }

        [Fact]
        public void GenerateToken_DifferentEmailsInSameStep_ReturnDifferentTokens()
        {
            var request1 = new ExistingCandidateRequest { Email = "email1@address.com", FirstName = "John", LastName = "Doe" };
            var request2 = new ExistingCandidateRequest { Email = "email2@address.com", FirstName = "John", LastName = "Doe" };
            var token1 = _service.GenerateToken(request1);
            var token2 = _service.GenerateToken(request2);
            token1.Should().NotBe(token2);
        }

        [Fact]
        public void GenerateToken_DifferentFirstNameInSameStep_ReturnDifferentTokens()
        {
            var request1 = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John1", LastName = "Doe" };
            var request2 = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John2", LastName = "Doe" };
            var token1 = _service.GenerateToken(request1);
            var token2 = _service.GenerateToken(request2);
            token1.Should().NotBe(token2);
        }

        [Fact]
        public void GenerateToken_SameRequestInSameStep_ReturnSameToken()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            var token1 = _service.GenerateToken(request);
            var token2 = _service.GenerateToken(request);
            token1.Should().Be(token2);
        }

        [Theory]
        [InlineData("000000")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("abcdef")]
        public void IsValid_WithInvalidToken_ReturnsFalse(string invalidToken)
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _service.IsValid(invalidToken, request).Should().BeFalse();
        }


        [Fact]
        public void IsValid_WithExpiredToken_ReturnsFalse()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            var secondsToOutsideOfWindow = CandidateAccessTokenService.StepInSeconds * (2 * CandidateAccessTokenService.VerificationWindow);
            var dateTimeOutsideOfWindow = DateTime.UtcNow.AddSeconds(-secondsToOutsideOfWindow);
            var token = _service.GenerateToken(request);
            _service.IsValid(token, request, dateTimeOutsideOfWindow).Should().BeFalse();
        }
    }
}
