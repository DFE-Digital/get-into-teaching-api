using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using GetIntoTeachingApiTests.Utils;
using GetIntoTeachingApi.Services;
using System.Collections.Generic;
using GetIntoTeachingApi.Services.Crm;

namespace GetIntoTeachingApiTests.Controllers
{
    public class CandidatesControllerTests
    {
        private readonly Mock<ILogger<CandidatesController>> _mockLogger;
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Mock<IWebApiClient> _mockClient;
        private readonly CandidatesController _controller;

        public CandidatesControllerTests()
        {
            _mockLogger = new Mock<ILogger<CandidatesController>>();
            _mockTokenService = new Mock<ICandidateAccessTokenService>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockClient = new Mock<IWebApiClient>();
            _controller = new CandidatesController(_mockLogger.Object, _mockTokenService.Object, _mockNotifyService.Object, _mockClient.Object);
        }

        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(CandidatesController));
        }

        [Fact]
        public async void CreateAccessToken_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new ExistingCandidateRequest { Email = "invalid-email@" };
            _controller.ModelState.AddModelError("Email", "Email is invalid.");

            var response = await _controller.CreateAccessToken(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Email").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("Email is invalid.");
        }

        [Fact]
        public async void CreateAccessToken_ValidRequest_SendsPINCodeEmail()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            var candidate = new Candidate { Email = request.Email, FirstName = request.FirstName, LastName = request.LastName };
            _mockTokenService.Setup(mock => mock.GenerateToken(request)).Returns("123456");
            _mockClient.Setup(mock => mock.GetCandidate(request)).ReturnsAsync(candidate);

            var response = await _controller.CreateAccessToken(request);

            response.Should().BeOfType<NoContentResult>();
            _mockNotifyService.Verify(
                mock => mock.SendEmail(
                    "email@address.com",
                    NotifyService.NewPinCodeEmailTemplateId,
                    It.Is<Dictionary<string, dynamic>>(personalisation => personalisation["pin_code"] as string == "123456")
                )
            );
        }

        [Fact]
        public async void CreateAccessToken_MismatchedCandidate_ReturnsNotFound()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockClient.Setup(mock => mock.GetCandidate(request)).ReturnsAsync((Candidate) null);

            var response = await _controller.CreateAccessToken(request);

            response.Should().BeOfType<NotFoundResult>();
            _mockNotifyService.Verify(mock => 
                mock.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>()), 
                Times.Never()
            );
        }
    }
}
