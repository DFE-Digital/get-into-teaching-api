using System;
using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;
using Moq;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Attributes;

namespace GetIntoTeachingApiTests.Controllers
{
    public class MailingListControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockAccessTokenService;
        private readonly Mock<ICandidateMagicLinkTokenService> _mockMagicLinkTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly MailingListController _controller;
        private readonly ExistingCandidateRequest _request;

        public MailingListControllerTests()
        {
            _request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockAccessTokenService = new Mock<ICandidateAccessTokenService>();
            _mockMagicLinkTokenService = new Mock<ICandidateMagicLinkTokenService>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _controller = new MailingListController(
                _mockAccessTokenService.Object,
                _mockMagicLinkTokenService.Object,
                _mockCrm.Object,
                _mockJobClient.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(MailingListController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,GetIntoTeaching");
        }

        [Fact]
        public void LogRequests_IsPresent()
        {
            typeof(MailingListController).Should().BeDecoratedWith<LogRequestsAttribute>();
        }

        [Fact]
        public void ExchangeAccessTokenForMember_MissingCandidate_RespondsWithUnauthorized()
        {
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns<Candidate>(null);

            var response = _controller.ExchangeAccessTokenForMember("000000", _request);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void ExchangeAccessTokenForMember_InvalidAccessToken_RespondsWithUnauthorized()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);
            _mockAccessTokenService.Setup(mock => mock.IsValid("000000", _request, (Guid)candidate.Id)).Returns(false);

            var response = _controller.ExchangeAccessTokenForMember("000000", _request);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void ExchangeAccessTokenForMember_ValidToken_RespondsWithMailingListAddMember()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockAccessTokenService.Setup(tokenService => tokenService.IsValid("000000", _request, (Guid)candidate.Id)).Returns(true);
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);

            var response = _controller.ExchangeAccessTokenForMember("000000", _request);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var responseModel = ok.Value as MailingListAddMember;
            responseModel.CandidateId.Should().Be(candidate.Id);
        }

        [Fact]
        public void ExchangeMagicLinkTokenForMember_MissingCandidate_RespondsWithUnauthorized()
        {
            var token = Guid.NewGuid().ToString();
            _mockMagicLinkTokenService.Setup(m => m.Exchange(token)).Returns(null as Candidate);

            var response = _controller.ExchangeMagicLinkTokenForMember(token);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void ExchangeMagicLinkTokenForMember_MatchingCandidate_RespondsWithMailingListAddMember()
        {
            var candidate = new Candidate { Id = Guid.NewGuid(), MagicLinkToken = Guid.NewGuid().ToString() };
            _mockMagicLinkTokenService.Setup(m => m.Exchange(candidate.MagicLinkToken)).Returns(candidate);

            var response = _controller.ExchangeMagicLinkTokenForMember(candidate.MagicLinkToken);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var responseModel = ok.Value as MailingListAddMember;
            responseModel.CandidateId.Should().Be(candidate.Id);
        }

        [Fact]
        public void AddMember_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new MailingListAddMember() { FirstName = null };
            _controller.ModelState.AddModelError("FirstName", "First name must be specified.");

            var response = _controller.AddMember(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("FirstName").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("First name must be specified.");
        }

        [Fact]
        public void AddMember_ValidRequest_EnqueuesJobRespondsWithNoContent()
        {
            var request = new MailingListAddMember() { Email = "test@test.com", FirstName = "John", LastName = "Doe" };

            var response = _controller.AddMember(request);

            response.Should().BeOfType<NoContentResult>();
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" && 
                IsMatch(request.Candidate, (Candidate)job.Args[0])),
                It.IsAny<EnqueuedState>()));
        }

        private static bool IsMatch(Candidate candidateA, Candidate candidateB)
        {
            // Compares ignoring date attributes that are dynamic.
            candidateA.Should().BeEquivalentTo(candidateB, options => options
                .Excluding(c => c.MailingListSubscriptionStartAt)
                .Excluding(c => c.EventsSubscriptionStartAt)
                .Excluding(c => c.PrivacyPolicy.AcceptedAt));
            return true;
        }
    }
}
