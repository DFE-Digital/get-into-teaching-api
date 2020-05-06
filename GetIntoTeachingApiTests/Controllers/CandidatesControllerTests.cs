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

namespace GetIntoTeachingApiTests.Controllers
{
    public class CandidatesControllerTests
    {
        private readonly Mock<ILogger<CandidatesController>> _mockLogger;
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly CandidatesController _controller;

        public CandidatesControllerTests()
        {
            _mockLogger = new Mock<ILogger<CandidatesController>>();
            _mockTokenService = new Mock<ICandidateAccessTokenService>();
            _mockNotifyService = new Mock<INotifyService>();
            _controller = new CandidatesController(_mockLogger.Object, _mockTokenService.Object, _mockNotifyService.Object);
        }

        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(CandidatesController));
        }

        [Fact]
        public void CreateAccessToken_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new CandidateAccessTokenRequest { Email = "invalid-email@" };
            _controller.ModelState.AddModelError("Email", "Email is invalid.");

            var response = _controller.CreateAccessToken(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Email").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("Email is invalid.");
        }

        [Fact]
        public void CreateAccessToken_ValidRequest_SendsPINCodeEmail()
        {
            var request = new CandidateAccessTokenRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockTokenService.Setup(mock => mock.GenerateToken("email@address.com")).Returns("123456");

            var response = _controller.CreateAccessToken(request);

            response.Should().BeOfType<NoContentResult>();
            _mockNotifyService.Verify(
                mock => mock.SendEmail(
                    "email@address.com", 
                    NotifyService.NewPinCodeEmailTemplateId, 
                    It.Is<Dictionary<string, dynamic>>(personalisation => personalisation["pin_code"] as string == "123456")
                )
            );
        }
    }
}
