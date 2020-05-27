using System;
using System.Collections.Generic;
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
        public async void Upsert_InvalidRequest_RespondsWithValidationErrors()
        {
            var candidate = new Candidate { Email = "invalid-email@" };
            _controller.ModelState.AddModelError("Email", "Email is invalid.");

            var response = await _controller.Upsert(candidate);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Email").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("Email is invalid.");
        }

        [Fact]
        public async void Upsert_ValidRequest_SavesAndRespondsWithSuccess()
        {
            var qualifications = new List<CandidateQualification> { new CandidateQualification() { CategoryId = 123 } };
            var positions = new List<CandidatePastTeachingPosition> { new CandidatePastTeachingPosition() { EducationPhaseId = 456 } };
            var phoneCall = new PhoneCall() { Telephone = "08574 857 364" };
            var privacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicy = new PrivacyPolicy() };
            var candidate = new Candidate
            {
                Qualifications = qualifications,
                PastTeachingPositions = positions,
                PhoneCall = phoneCall,
                PrivacyPolicy = privacyPolicy
            };

            _mockClient.Setup(mock => mock.Upsert(qualifications)).ReturnsAsync(qualifications);
            _mockClient.Setup(mock => mock.Upsert(positions)).ReturnsAsync(positions);
            _mockClient.Setup(mock => mock.Upsert(phoneCall)).ReturnsAsync(phoneCall);
            _mockClient.Setup(mock => mock.Upsert(privacyPolicy)).ReturnsAsync(privacyPolicy);
            _mockClient.Setup(mock => mock.Upsert(candidate)).ReturnsAsync(candidate);

            var response = await _controller.Upsert(candidate);

            response.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async void Upsert_WithNullPhoneCallAndPrivacyPolicy_SavesAndRespondsWithSuccess()
        {
            var qualifications = new List<CandidateQualification> { new CandidateQualification() { CategoryId = 123 } };
            var positions = new List<CandidatePastTeachingPosition> { new CandidatePastTeachingPosition() { EducationPhaseId = 456 } };
            var candidate = new Candidate
            {
                Qualifications = qualifications,
                PastTeachingPositions = positions,
            };

            _mockClient.Setup(mock => mock.Upsert(qualifications)).ReturnsAsync(qualifications);
            _mockClient.Setup(mock => mock.Upsert(positions)).ReturnsAsync(positions);
            _mockClient.Setup(mock => mock.Upsert(candidate)).ReturnsAsync(candidate);

            var response = await _controller.Upsert(candidate);

            response.Should().BeOfType<NoContentResult>();
            _mockClient.Verify(mock => mock.Upsert(It.IsAny<PrivacyPolicy>()), Times.Never);
            _mockClient.Verify(mock => mock.Upsert(It.IsAny<PhoneCall>()), Times.Never);
        }
    }
}
