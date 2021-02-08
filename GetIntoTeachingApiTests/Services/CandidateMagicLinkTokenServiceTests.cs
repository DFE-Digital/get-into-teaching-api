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
            _service = new CandidateMagicLinkTokenService(_mockCrm.Object);
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
        public void Exchange_WithValidToken_ReturnsCandidateAndUpdatesStatus()
        {
            var candidate = new Candidate();
            var token = Guid.NewGuid().ToString();
            _mockCrm.Setup(m => m.MatchCandidates(token)).Returns(new Candidate[] { candidate });

            var result = _service.Exchange(token);

            result.Should().Be(candidate);
            candidate.MagicLinkTokenStatusId.Should().Be((int)Candidate.MagicLinkTokenStatus.Exchanged);
        }

        [Fact]
        public void Exchange_WhenTokenMatchesMultipleCandidates_ReturnsNull()
        {
            var candidates = new List<Candidate>() { new Candidate(), new Candidate() };
            var token = Guid.NewGuid().ToString();
            _mockCrm.Setup(m => m.MatchCandidates(token)).Returns(candidates);

            var result = _service.Exchange(token);

            result.Should().BeNull();
        }

        [Fact]
        public void Exchange_WhenTokenDoesNotMatch_ReturnsNull()
        {
            var token = Guid.NewGuid().ToString();
            _mockCrm.Setup(m => m.MatchCandidates(token)).Returns(new Candidate[0]);

            var result = _service.Exchange(token);

            result.Should().BeNull();
        }
    }
}
