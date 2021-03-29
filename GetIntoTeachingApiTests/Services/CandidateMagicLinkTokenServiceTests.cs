using System;
using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class CandidateMagicLinkTokenServiceTests
    {
        private readonly Mock<ICrmService> _mockCrm;
        private readonly CandidateMagicLinkTokenService _service;

        public CandidateMagicLinkTokenServiceTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _service = new CandidateMagicLinkTokenService(_mockCrm.Object, new DateTimeProvider());
        }

        [Fact]
        public void GenerateTokens_WithCandidates_SetsTokenDetails()
        {
            var candidate = new Candidate();

            _service.GenerateToken(candidate);

            candidate.MagicLinkToken.Should().NotBeNull();
            candidate.MagicLinkToken.Length.Should().Be(32);
            candidate.MagicLinkTokenExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(48));
            candidate.MagicLinkTokenStatusId.Should().Be((int)Candidate.MagicLinkTokenStatus.Generated);
        }

        [Fact]
        public void GenerateTokens_WhenCalledMultipleTimes_CreatesUniqueTokens()
        {
            var candidate = new Candidate();
            var tokens = new List<string>();

            _service.GenerateToken(candidate);
            tokens.Add(candidate.MagicLinkToken);
            _service.GenerateToken(candidate);
            tokens.Add(candidate.MagicLinkToken);
            _service.GenerateToken(candidate);
            tokens.Add(candidate.MagicLinkToken);

            tokens.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void Exchange_WithValidToken_ReturnsSuccessAndUpdatesMagicLinkTokenStatusId()
        {
            var candidate = new Candidate() { MagicLinkTokenExpiresAt = DateTime.UtcNow.AddMinutes(1) };
            var token = Guid.NewGuid().ToString();
            _mockCrm.Setup(m => m.MatchCandidates(token)).Returns(new Candidate[] { candidate });

            var result = _service.Exchange(token);

            result.Success.Should().BeTrue();
            result.Candidate.Should().Be(candidate);
            result.Status.Should().Be(CandidateMagicLinkExchangeResult.ExchangeStatus.Valid);
            candidate.MagicLinkTokenStatusId.Should().Be((int)Candidate.MagicLinkTokenStatus.Exchanged);
        }

        [Fact]
        public void Exchange_WithExpiredToken_ReturnsFailure()
        {
            var candidate = new Candidate() { MagicLinkTokenExpiresAt = DateTime.UtcNow.AddMinutes(-1) };
            var token = Guid.NewGuid().ToString();
            _mockCrm.Setup(m => m.MatchCandidates(token)).Returns(new Candidate[] { candidate });

            var result = _service.Exchange(token);

            result.Success.Should().BeFalse();
            result.Candidate.Should().Be(candidate);
            result.Status.Should().Be(CandidateMagicLinkExchangeResult.ExchangeStatus.Expired);
        }

        [Fact]
        public void Exchange_WithAlreadyExchangedToken_ReturnsFailure()
        {
            var candidate = new Candidate() { MagicLinkTokenStatusId = (int)Candidate.MagicLinkTokenStatus.Exchanged };
            var token = Guid.NewGuid().ToString();
            _mockCrm.Setup(m => m.MatchCandidates(token)).Returns(new Candidate[] { candidate });

            var result = _service.Exchange(token);

            result.Success.Should().BeFalse();
            result.Candidate.Should().Be(candidate);
            result.Status.Should().Be(CandidateMagicLinkExchangeResult.ExchangeStatus.AlreadyExchanged);
        }

        [Fact]
        public void Exchange_WhenTokenMatchesMultipleCandidates_ReturnsFailure()
        {
            var candidates = new List<Candidate>() { new Candidate(), new Candidate() };
            var token = Guid.NewGuid().ToString();
            _mockCrm.Setup(m => m.MatchCandidates(token)).Returns(candidates);

            var result = _service.Exchange(token);

            result.Success.Should().BeFalse();
            result.Candidate.Should().BeNull();
            result.Status.Should().Be(CandidateMagicLinkExchangeResult.ExchangeStatus.Invalid);
        }

        [Fact]
        public void Exchange_WhenTokenDoesNotMatch_ReturnsFailure()
        {
            var token = Guid.NewGuid().ToString();
            _mockCrm.Setup(m => m.MatchCandidates(token)).Returns(new Candidate[0]);

            var result = _service.Exchange(token);

            result.Success.Should().BeFalse();
            result.Candidate.Should().BeNull();
            result.Status.Should().Be(CandidateMagicLinkExchangeResult.ExchangeStatus.Invalid);
        }
    }
}
