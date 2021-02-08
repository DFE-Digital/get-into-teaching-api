using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;
using static GetIntoTeachingApi.Models.CandidateMagicLinkExchangeResult;

namespace GetIntoTeachingApiTests.Models
{
    public class CandidateMagicLinkExchangeResultTests
    {
        [Fact]
        public void Status_WithNullCandidate_ReturnsInvalid()
        {
            var result = new CandidateMagicLinkExchangeResult(null);

            result.Status.Should().Be(ExchangeStatus.Invalid);
        }

        [Fact]
        public void Status_WhenCandidateHasExpiredToken_ReturnsExpired()
        {
            var candidate = new Candidate() { MagicLinkTokenExpiresAt = DateTime.UtcNow.AddMinutes(-1) };

            var result = new CandidateMagicLinkExchangeResult(candidate);

            result.Status.Should().Be(ExchangeStatus.Expired);
        }

        [Fact]
        public void Status_WhenCandidateHasAlreadyExchangedToken_ReturnsAlreadyExchanged()
        {
            var candidate = new Candidate() { MagicLinkTokenStatusId = (int)Candidate.MagicLinkTokenStatus.Exchanged };

            var result = new CandidateMagicLinkExchangeResult(candidate);

            result.Status.Should().Be(ExchangeStatus.AlreadyExchanged);
        }

        [Fact]
        public void Status_WhenCandidateHasValidToken_ReturnsValid()
        {
            var candidate = new Candidate() { MagicLinkTokenExpiresAt = DateTime.UtcNow.AddMinutes(1) };

            var result = new CandidateMagicLinkExchangeResult(candidate);

            result.Status.Should().Be(ExchangeStatus.Valid);
        }

        [Fact]
        public void Success_WhenCandidateHasValidToken_ReturnsTrue()
        {
            var candidate = new Candidate() { MagicLinkTokenExpiresAt = DateTime.UtcNow.AddMinutes(1) };

            var result = new CandidateMagicLinkExchangeResult(candidate);

            result.Success.Should().BeTrue();
        }

        [Fact]
        public void Success_WhenCandidateHasInvalidToken_ReturnsFalse()
        {
            var result = new CandidateMagicLinkExchangeResult(null);

            result.Success.Should().BeFalse();
        }
    }
}
