using System;
using Xunit;
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
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.TeacherTrainingAdviser;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApiTests.Controllers.TeacherTrainingAdviser
{
    public class CandidatesControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<IDateTimeProvider> _mockDateTime;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<ILogger<CandidatesController>> _mockLogger;
        private readonly CandidatesController _controller;
        private readonly ExistingCandidateRequest _request;

        public CandidatesControllerTests()
        {
            _mockTokenService = new Mock<ICandidateAccessTokenService>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _mockDateTime = new Mock<IDateTimeProvider>();
            _mockLogger = new Mock<ILogger<CandidatesController>>();
            _mockAppSettings = new Mock<IAppSettings>();
            _request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _controller = new CandidatesController(_mockTokenService.Object, _mockCrm.Object,
                _mockJobClient.Object, _mockDateTime.Object, _mockAppSettings.Object, _mockLogger.Object);
            _controller.MockUser("TTA");

            // Freeze time.
            _mockDateTime.Setup(m => m.UtcNow).Returns(DateTime.UtcNow);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(CandidatesController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,GetAnAdviser,Apply");
        }

        [Fact]
        public void ExchangeAccessToken_InvalidAccessToken_RespondsWithUnauthorized()
        {
            _request.Reference = "Ref";
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);
            _mockTokenService.Setup(mock => mock.IsValid("000000", _request, (Guid)candidate.Id)).Returns(false);

            var response = _controller.ExchangeAccessToken("000000", _request);

            _request.Reference.Should().Be("Ref");
            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void ExchangeAccessToken_ValidToken_RespondsWithTeacherTrainingAdviserSignUp()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request, (Guid)candidate.Id)).Returns(true);
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);

            var response = _controller.ExchangeAccessToken("000000", _request);

            _request.Reference.Should().Be("TTA");
            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var responseModel = ok.Value as TeacherTrainingAdviserSignUp;
            responseModel.CandidateId.Should().Be(candidate.Id);
        }

        [Fact]
        public void ExchangeAccessToken_MissingCandidate_RespondsWithUnauthorized()
        {
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns<Candidate>(null);

            var response = _controller.ExchangeAccessToken("000000", _request);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void SignUp_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new TeacherTrainingAdviserSignUp { Email = "invalid-email@" };
            _controller.ModelState.AddModelError("Email", "Email is invalid.");

            var response = _controller.SignUp(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Email").WhoseValue.Should().BeOfType<string[]>().Which.Should().Contain("Email is invalid.");
        }

        [Fact]
        public void SignUp_ValidRequest_EnqueuesJobAndRespondsWithSuccess()
        {
            var request = new TeacherTrainingAdviserSignUp { FirstName = "first" };

            var response = _controller.SignUp(request);

            response.Should().BeOfType<NoContentResult>();
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                IsMatch(request.Candidate, (string)job.Args[0])),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Matchback_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new ExistingCandidateRequest { Email = "invalid-email@", Reference = "Ref" };
            _controller.ModelState.AddModelError("Email", "Email is invalid.");

            var response = _controller.Matchback(request);

            request.Reference.Should().Be("Ref");
            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Email").WhoseValue.Should().BeOfType<string[]>().Which.Should().Contain("Email is invalid.");
        }

        [Fact]
        public void Matchback_ValidRequest_ReturnsCandidateMatchbackResponse()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            var candidate = new Candidate { Id = Guid.NewGuid(), Email = request.Email, FirstName = request.FirstName, LastName = request.LastName };
            _mockCrm.Setup(mock => mock.MatchCandidate(request)).Returns(candidate);
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);

            var response = _controller.Matchback(request);

            request.Reference.Should().Be("TTA");
            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var matchbackResponse = (TeacherTrainingAdviserSignUp)ok.Value;
            matchbackResponse.CandidateId.Should().Be((Guid)candidate.Id);
        }

        [Fact]
        public void Matchback_MismatchedCandidate_ReturnsNotFound()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockCrm.Setup(mock => mock.MatchCandidate(request)).Returns<Candidate>(null);
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);

            var response = _controller.Matchback(request);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Matchback_CrmThrowsException_LogsAndReRaises()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockCrm.Setup(m => m.MatchCandidate(request)).Throws(new Exception("Error"));
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);

            _controller.Invoking(c => c.Matchback(request))
                .Should().Throw<Exception>()
                .WithMessage("Error");

            _mockLogger.VerifyInformationWasCalled("TeacherTrainingAdviser - CandidatesController - potential duplicate (CRM exception) - Error");
        }

        [Fact]
        public void Matchback_CrmIntegrationIsPaused_ReturnsNotFound()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);

            var response = _controller.Matchback(request);

            response.Should().BeOfType<NotFoundResult>();

            _mockLogger.VerifyInformationWasCalled("TeacherTrainingAdviser - CandidatesController - potential duplicate (CRM integration paused)");
        }

        private static bool IsMatch(Candidate candidateA, string candidateBJson)
        {
            var candidateB = candidateBJson.DeserializeChangeTracked<Candidate>();
            candidateA.Should().BeEquivalentTo(candidateB);
            return true;
        }
    }
}
