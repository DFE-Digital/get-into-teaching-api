using System;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Controllers.TeacherTrainingAdviser;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using Microsoft.AspNetCore.Mvc;
using GetIntoTeachingApi.Models;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;

namespace GetIntoTeachingApiTests.Controllers.TeacherTrainingAdviser
{
    public class CandidatesControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly CandidatesController _controller;
        private readonly ExistingCandidateRequest _request;

        public CandidatesControllerTests()
        {
            _mockTokenService = new Mock<ICandidateAccessTokenService>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            var mockLogger = new Mock<ILogger<CandidatesController>>();
            _controller = new CandidatesController(mockLogger.Object, _mockTokenService.Object, _mockCrm.Object, _mockJobClient.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(CandidatesController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void Get_InvalidAccessToken_RespondsWithUnauthorized()
        {
            _mockTokenService.Setup(mock => mock.IsValid("000000", _request)).Returns(false);

            var response = _controller.Get("000000", _request);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void Get_ValidToken_RespondsWithCandidate()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request)).Returns(true);
            _mockCrm.Setup(mock => mock.GetCandidate(_request)).Returns(candidate);

            var response = _controller.Get("000000", _request);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var candidateResponse = ok.Value as Candidate;
            candidateResponse.Should().Be(candidate);
        }

        [Fact]
        public void Get_MissingCandidate_RespondsWithNotFound()
        {
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request)).Returns(true);
            _mockCrm.Setup(mock => mock.GetCandidate(_request)).Returns<Candidate>(null);

            var response = _controller.Get("000000", _request);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Upsert_InvalidRequest_RespondsWithValidationErrors()
        {
            var candidate = new Candidate { Email = "invalid-email@" };
            _controller.ModelState.AddModelError("Email", "Email is invalid.");

            var response = _controller.Upsert(candidate);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Email").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("Email is invalid.");
        }

        [Fact]
        public void Upsert_ValidRequest_EnqueuesJobAndRespondsWithSuccess()
        {
            var candidate = new Candidate { FirstName = "first" };

            var response = _controller.Upsert(candidate);

            response.Should().BeOfType<NoContentResult>();
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(CandidateRegistrationJob) && job.Method.Name == "Run" && job.Args[0] == candidate),
                It.IsAny<EnqueuedState>()));
        }
    }
}
