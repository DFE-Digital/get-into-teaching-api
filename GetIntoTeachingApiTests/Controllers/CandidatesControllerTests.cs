using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using GetIntoTeachingApi.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.Extensions.Logging;
using GetIntoTeachingApiTests.Helpers;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApiTests.Controllers
{
    public class CandidatesControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockAccessTokenService;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<ILogger<CandidatesController>> _mockLogger;
        private readonly CandidatesController _controller;

        public CandidatesControllerTests()
        {
            _mockAccessTokenService = new Mock<ICandidateAccessTokenService>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockAppSettings = new Mock<IAppSettings>();
            _mockCrm = new Mock<ICrmService>();
            _mockLogger = new Mock<ILogger<CandidatesController>>();
            _controller = new CandidatesController(
                _mockAccessTokenService.Object,
                _mockNotifyService.Object,
                _mockCrm.Object,
                _mockAppSettings.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(CandidatesController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,GetIntoTeaching,GetAnAdviser,SchoolsExperience");
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
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);

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
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);

            var response = _controller.CreateAccessToken(request);

            response.Should().BeOfType<NotFoundResult>();

            _mockNotifyService.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>()),
                Times.Never()
            );
        }

        [Fact]
        public void CreateAccessToken_CrmThrowsException_ReturnsNotFound()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockCrm.Setup(m => m.MatchCandidate(request)).Throws(new Exception("Error"));
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);

            var response = _controller.CreateAccessToken(request);

            response.Should().BeOfType<NotFoundResult>();

            _mockNotifyService.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>()),
                Times.Never()
            );
            _mockLogger.VerifyInformationWasCalled("CandidatesController - potential duplicate (CRM exception) - Error");
        }

        [Fact]
        public void CreateAccessToken_CrmIntegrationIsPaused_ReturnsNotFound()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);

            var response = _controller.CreateAccessToken(request);

            response.Should().BeOfType<NotFoundResult>();

            _mockNotifyService.Verify(mock =>
                mock.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>()),
                Times.Never()
            );
            _mockLogger.VerifyInformationWasCalled("CandidatesController - potential duplicate (CRM integration paused)");
        }
    }
}
