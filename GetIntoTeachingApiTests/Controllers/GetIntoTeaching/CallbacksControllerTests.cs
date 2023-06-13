﻿using System;
using Xunit;
using Moq;
using GetIntoTeachingApi.Services;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using Microsoft.AspNetCore.Mvc;
using GetIntoTeachingApi.Models;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApi.Controllers.GetIntoTeaching;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApiTests.Helpers;

namespace GetIntoTeachingApiTests.Controllers.GetIntoTeaching
{
    public class CallbacksControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<IDateTimeProvider> _mockDateTime;
        private readonly CallbacksController _controller;
        private readonly ExistingCandidateRequest _request;

        public CallbacksControllerTests()
        {
            _mockTokenService = new Mock<ICandidateAccessTokenService>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _mockDateTime = new Mock<IDateTimeProvider>();
            _request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _controller = new CallbacksController(_mockTokenService.Object, _mockCrm.Object, _mockDateTime.Object, _mockJobClient.Object);
            _controller.MockUser("GIT");

            // Freeze time.
            _mockDateTime.Setup(m => m.UtcNow).Returns(DateTime.UtcNow);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(CallbacksController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,GetIntoTeaching");
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
        public void ExchangeAccessToken_ValidToken_RespondsWithGetIntoTeachingCallback()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request, (Guid)candidate.Id)).Returns(true);
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);

            var response = _controller.ExchangeAccessToken("000000", _request);

            _request.Reference.Should().Be("GIT");
            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var responseModel = ok.Value as GetIntoTeachingCallback;
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
        public void Book_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new GetIntoTeachingCallback { Email = "invalid-email@" };
            _controller.ModelState.AddModelError("Email", "Email is invalid.");

            var response = _controller.Book(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Email").WhoseValue.Should().BeOfType<string[]>().Which.Should().Contain("Email is invalid.");
        }

        [Fact]
        public void Book_ValidRequest_EnqueuesJobAndRespondsWithSuccess()
        {
            var request = new GetIntoTeachingCallback { FirstName = "first", PhoneCallScheduledAt = DateTime.UtcNow };

            var response = _controller.Book(request);

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

            var response = _controller.Matchback(request);

            request.Reference.Should().Be("GIT");
            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var matchbackResponse = (GetIntoTeachingCallback)ok.Value;
            matchbackResponse.CandidateId.Should().Be((Guid)candidate.Id);
        }

        [Fact]
        public void Matchback_MismatchedCandidate_ReturnsNotFound()
        {
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockCrm.Setup(mock => mock.MatchCandidate(request)).Returns<Candidate>(null);

            var response = _controller.Matchback(request);

            response.Should().BeOfType<NotFoundResult>();
        }

        private static bool IsMatch(Candidate candidateA, string candidateBJson)
        {
            var candidateB = candidateBJson.DeserializeChangeTracked<Candidate>();
            candidateA.Should().BeEquivalentTo(candidateB);
            return true;
        }
    }
}
