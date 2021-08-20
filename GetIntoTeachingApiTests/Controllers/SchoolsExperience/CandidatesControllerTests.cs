using System;
using Xunit;
using Moq;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Controllers.SchoolsExperience;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using Microsoft.AspNetCore.Mvc;
using GetIntoTeachingApi.Models;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;
using GetIntoTeachingApi.Utils;
using System.Linq;
using System.Collections.Generic;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.SchoolsExperience;

namespace GetIntoTeachingApiTests.Controllers.SchoolsExperience
{
    public class CandidatesControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<ICandidateUpserter> _mockUpserter;
        private readonly CandidatesController _controller;
        private readonly ExistingCandidateRequest _request;

        public CandidatesControllerTests()
        {
            _mockTokenService = new Mock<ICandidateAccessTokenService>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _mockUpserter = new Mock<ICandidateUpserter>();
            _request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _controller = new CandidatesController(
                _mockTokenService.Object,
                _mockCrm.Object,
                _mockUpserter.Object,
                _mockJobClient.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(CandidatesController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,SchoolsExperience");
        }

        [Fact]
        public void ExchangeAccessToken_InvalidAccessToken_RespondsWithUnauthorized()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);
            _mockTokenService.Setup(mock => mock.IsValid("000000", _request, (Guid)candidate.Id)).Returns(false);

            var response = _controller.ExchangeAccessToken("000000", _request);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void ExchangeAccessToken_ValidToken_RespondsWithSchoolsExperienceSignUp()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request, (Guid)candidate.Id)).Returns(true);
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);

            var response = _controller.ExchangeAccessToken("000000", _request);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var responseModel = ok.Value as SchoolsExperienceSignUp;
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
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            var request = new SchoolsExperienceSignUp { Email = "invalid-email@" };
            _controller.ModelState.AddModelError("Email", "Email is invalid.");

            var response = _controller.SignUp(request, mockAppSettings.Object);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Email").WhoseValue.Should().BeOfType<string[]>().Which.Should().Contain("Email is invalid.");
        }

        [Fact]
        public void SignUp_ValidRequest_UpsertsCandidateAndRespondsWithSuccess()
        {
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
            var request = new SchoolsExperienceSignUp { FirstName = "first" };
            var candidateId = Guid.NewGuid();
            _mockUpserter.Setup(m => m.Upsert(It.IsAny<Candidate>())).Callback<Candidate>((c) => c.Id = candidateId);

            var response = _controller.SignUp(request, mockAppSettings.Object);

            var created = response.Should().BeOfType<CreatedAtActionResult>().Subject;
            var signUp = created.Value.Should().BeAssignableTo<SchoolsExperienceSignUp>().Subject;
            signUp.FirstName.Should().Be(request.FirstName);
            signUp.CandidateId.Should().Be(candidateId);

            _mockUpserter.Verify(m => m.Upsert(It.IsAny<Candidate>()), Times.Once);
        }

        [Fact]
        public void SignUp_WhenCrmIntegrationIsPaused_RaisesError()
        {
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);
            var request = new SchoolsExperienceSignUp { FirstName = "first" };

            _controller.Invoking(c => c.SignUp(request, mockAppSettings.Object))
                .Should().Throw<InvalidOperationException>()
                .WithMessage("CandidatesController#SignUp - Aborting (CRM integration paused).");
        }

        [Fact]
        public void Get_WhenFound_ReturnsSchoolsExperienceSignUp()
        {
            var candidate = new Candidate() { Id = Guid.NewGuid() };
            _mockCrm.Setup(mock => mock.GetCandidate((Guid)candidate.Id)).Returns(candidate);

            var response = _controller.Get((Guid)candidate.Id);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var signUp = ok.Value.Should().BeAssignableTo<SchoolsExperienceSignUp>().Subject;
            signUp.CandidateId.Should().Be(candidate.Id);
        }

        [Fact]
        public void Get_WhenNotFound_ReturnsNotFound()
        {
            _mockCrm.Setup(mock => mock.GetCandidate(It.IsAny<Guid>())).Returns(null as Candidate);

            var response = _controller.Get(Guid.NewGuid());

            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void GetMultiple_WithIds_ReturnsSchoolsExperienceSignUps()
        {
            var candidates = new Candidate[]
            {
                new Candidate() { Id = Guid.NewGuid() },
                new Candidate() { Id = Guid.NewGuid() },
            };
            var ids = candidates.Select(c => (Guid)c.Id);
            _mockCrm.Setup(mock => mock.GetCandidates(ids)).Returns(candidates);

            var response = _controller.GetMultiple(ids);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var signUps = ok.Value.Should().BeAssignableTo<IEnumerable<SchoolsExperienceSignUp>>().Subject;
            signUps.Select(c => c.CandidateId).Should().BeEquivalentTo(ids);
        }

        [Fact]
        public void Get_WhenNotFound_ReturnsEmpty()
        {
            _mockCrm.Setup(mock => mock.GetCandidates(It.IsAny<IEnumerable<Guid>>())).Returns(new Candidate[0]);

            var response = _controller.GetMultiple(new Guid[] { Guid.NewGuid() });

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var signUps = ok.Value.Should().BeAssignableTo<IEnumerable<SchoolsExperienceSignUp>>().Subject;
            signUps.Should().BeEmpty();
        }

        [Fact]
        public void AddClassroomExperienceNote_InvalidRequest_RespondsWithValidationErrors()
        {
            var candidateId = Guid.NewGuid();
            var note = new ClassroomExperienceNote { SchoolName = null };
            _controller.ModelState.AddModelError("SchoolName", "SchoolName must be set.");

            var response = _controller.AddClassroomExperienceNote(candidateId, note);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("SchoolName").WhoseValue.Should().BeOfType<string[]>().Which.Should().Contain("SchoolName must be set.");
        }

        [Fact]
        public void AddClassroomExperienceNote_CandidateNotFound_RespondsWithNotFound()
        {
            var candidateId = Guid.NewGuid();
            var note = new ClassroomExperienceNote() { Action = "REQUESTED", SchoolName = "School Name", SchoolUrn = 123456 };
            _mockCrm.Setup(m => m.GetCandidate(candidateId)).Returns(null as Candidate);

            var response = _controller.AddClassroomExperienceNote(candidateId, note);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void AddClassroomExperienceNote_ValidRequest_EnqueuesJobAndRespondsWithSuccess()
        {
            var candidate = new Candidate() { Id = Guid.NewGuid() };
            var note = new ClassroomExperienceNote() { Action = "REQUESTED", SchoolName = "School Name", SchoolUrn = 123456 };
            _mockCrm.Setup(m => m.GetCandidate((Guid)candidate.Id)).Returns(candidate);

            var response = _controller.AddClassroomExperienceNote((Guid)candidate.Id, note);

            candidate.AddClassroomExperienceNote(note);
            response.Should().BeOfType<NoContentResult>();
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                IsMatch(candidate, (string)job.Args[0])),
                It.IsAny<EnqueuedState>()));
        }

        private static bool IsMatch(Candidate candidateA, string candidateBJson)
        {
            var candidateB = candidateBJson.DeserializeChangeTracked<Candidate>();
            candidateA.Should().BeEquivalentTo(candidateB);
            return true;
        }
    }
}
