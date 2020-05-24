using System;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Controllers.TeacherTrainingAdviser;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services.Crm;
using GetIntoTeachingApiTests.Utils;

namespace GetIntoTeachingApiTests.Controllers.TeacherTrainingAdviser
{
    public class CandidatesControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IWebApiClient> _mockClient;
        private readonly CandidatesController _controller;
        private readonly ExistingCandidateRequest _request;

        public CandidatesControllerTests()
        {
            _mockTokenService = new Mock<ICandidateAccessTokenService>();
            _mockCrm = new Mock<ICrmService>();
            _mockClient = new Mock<IWebApiClient>();
            _request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            var mockLogger = new Mock<ILogger<CandidatesController>>();
            _controller = new CandidatesController(mockLogger.Object, _mockTokenService.Object, _mockCrm.Object, _mockClient.Object);
        }

        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(CandidatesController));
        }

        [Fact]
        public async void Get_InvalidAccessToken_RespondsWithUnauthorized()
        {
            _mockTokenService.Setup(mock => mock.IsValid("000000", _request)).Returns(false);

            var response = await _controller.Get("000000", _request);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async void Get_ValidToken_RespondsWithCandidate()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request)).Returns(true);
            _mockClient.Setup(mock => mock.GetCandidate(_request)).ReturnsAsync(candidate);

            var response = await _controller.Get("000000", _request);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var candidateResponse = ok.Value as Candidate;
            candidateResponse.Should().Be(candidate);
        }

        [Fact]
        public async void Get_MissingCandidate_RespondsWithNotFound()
        {
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request)).Returns(true);
            _mockClient.Setup(mock => mock.GetCandidate(_request)).ReturnsAsync((Candidate) null);

            var response = await _controller.Get("000000", _request);

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
        public void Upsert_ValidRequest_SavesAndRespondsWithSuccess()
        {
            var candidate = new Candidate { FirstName = "first" };
            _mockCrm.Setup(mock => mock.Save(candidate));

            var response = _controller.Upsert(candidate);

            response.Should().BeOfType<NoContentResult>();
        }
    }
}
