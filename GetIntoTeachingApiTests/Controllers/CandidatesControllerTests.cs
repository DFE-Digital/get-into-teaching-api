using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using GetIntoTeachingApi.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using GetIntoTeachingApi.Attributes;
using System;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using GetIntoTeachingApi.Jobs;
using System.Linq;

namespace GetIntoTeachingApiTests.Controllers
{
    public class CandidatesControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockAccessTokenService;
        private readonly Mock<ICandidateMagicLinkTokenService> _mockMagicLinkTokenService;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly CandidatesController _controller;

        public CandidatesControllerTests()
        {
            _mockAccessTokenService = new Mock<ICandidateAccessTokenService>();
            _mockMagicLinkTokenService = new Mock<ICandidateMagicLinkTokenService>() { CallBase = true };
            _mockNotifyService = new Mock<INotifyService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _mockCrm = new Mock<ICrmService>();
            _controller = new CandidatesController(
                _mockAccessTokenService.Object,
                _mockMagicLinkTokenService.Object,
                _mockNotifyService.Object,
                _mockCrm.Object,
                _mockJobClient.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(CandidatesController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,GetIntoTeaching,GetAnAdviser");
        }

        [Fact]
        public void LogRequests_IsPresent()
        {
            typeof(CandidatesController).Should().BeDecoratedWith<LogRequestsAttribute>();
        }

        [Fact]
        public void CreateAccessToken_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new ExistingCandidateRequest { Email = "invalid-email@" };
            _controller.ModelState.AddModelError("Email", "Email is invalid.");

            var response = _controller.CreateAccessToken(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Email").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("Email is invalid.");
        }

        [Fact]
        public void CreateAccessToken_ValidRequest_SendsPINCodeEmail()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            var candidate = new Candidate { Id = Guid.NewGuid(), Email = request.Email, FirstName = request.FirstName, LastName = request.LastName };
            _mockAccessTokenService.Setup(mock => mock.GenerateToken(request, (Guid)candidate.Id)).Returns("123456");
            _mockCrm.Setup(mock => mock.MatchCandidate(request)).Returns(candidate);

            var response = _controller.CreateAccessToken(request);

            response.Should().BeOfType<NoContentResult>();
            _mockNotifyService.Verify(
                mock => mock.SendEmailAsync(
                    "email@address.com",
                    NotifyService.NewPinCodeEmailTemplateId,
                    It.Is<Dictionary<string, dynamic>>(personalisation => personalisation["pin_code"] as string == "123456")
                )
            );
        }

        [Fact]
        public void CreateAccessToken_MismatchedCandidate_ReturnsNotFound()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockCrm.Setup(mock => mock.MatchCandidate(request)).Returns<Candidate>(null);

            var response = _controller.CreateAccessToken(request);

            response.Should().BeOfType<NotFoundResult>();

            _mockNotifyService.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>()),
                Times.Never()
            );
        }

        [Fact]
        public void CreateMagicLinkToken_TooManyCandidates_RespondsWithBadRequest()
        {
            var candidateIds = new Guid[26];

            var response = _controller.CreateMagicLinkToken(candidateIds);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("You can only generate 25 magic link tokens per request.");
        }

        [Fact]
        public void CreateMagicLinkToken_MissingCandidateId_RespondsWithBadRequest()
        {
            var foundCandidateId = Guid.NewGuid();
            var missingCandidateId = Guid.NewGuid();
            var candidates = new List<Candidate>() { new Candidate() { Id = foundCandidateId } };
            var candidate = candidates.First();
            var candidateIds = new Guid[] { foundCandidateId, missingCandidateId };
            _mockCrm.Setup(m => m.GetCandidates(candidateIds)).Returns(candidates);

            var response = _controller.CreateMagicLinkToken(candidateIds);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().BeEquivalentTo(new { message = "Candidate IDs could not be found.", missingCandidateIds = new Guid[] { missingCandidateId } });
        }

        [Fact]
        public void CreateMagicLinkToken_WithValidCandidateIds_GeneratesMagicLinkTokensAndUpdatesCandidate()
        {
            var candidates = new List<Candidate>() { new Candidate() { Id = Guid.NewGuid() } };
            var candidate = candidates.First();
            var candidateIds = candidates.Select(c => (Guid)c.Id);
            _mockCrm.Setup(m => m.GetCandidates(candidateIds)).Returns(candidates);

            var response = _controller.CreateMagicLinkToken(candidateIds);

            response.Should().BeOfType<NoContentResult>();

            _mockMagicLinkTokenService.Verify(m => m.GenerateToken(candidate));

            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                candidate == (Candidate)job.Args[0]),
                It.IsAny<EnqueuedState>()));
        }
    }
}
