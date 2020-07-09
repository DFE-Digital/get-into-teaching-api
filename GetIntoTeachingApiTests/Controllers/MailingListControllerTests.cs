﻿using System;
using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;
using Moq;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApiTests.Controllers
{
    public class MailingListControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly MailingListController _controller;
        private readonly ExistingCandidateRequest _request;

        public MailingListControllerTests()
        {
            _request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockTokenService = new Mock<ICandidateAccessTokenService>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _controller = new MailingListController(_mockTokenService.Object, _mockCrm.Object, _mockJobClient.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(MailingListController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void GetMember_InvalidAccessToken_RespondsWithUnauthorized()
        {
            _mockTokenService.Setup(mock => mock.IsValid("000000", _request)).Returns(false);

            var response = _controller.GetMember("000000", _request);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void GetMember_ValidToken_RespondsWithMailingListAddMember()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request)).Returns(true);
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);

            var response = _controller.GetMember("000000", _request);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var responseModel = ok.Value as MailingListAddMember;
            responseModel.CandidateId.Should().Be(candidate.Id);
        }

        [Fact]
        public void GetMember_MissingCandidate_RespondsWithNotFound()
        {
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request)).Returns(true);
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns<Candidate>(null);

            var response = _controller.GetMember("000000", _request);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void AddMember_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new MailingListAddMember() { FirstName = null };
            _controller.ModelState.AddModelError("FirstName", "First name must be specified.");

            var response = _controller.AddMember(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("FirstName").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("First name must be specified.");
        }

        [Fact]
        public void AddMember_ValidRequest_EnqueuesJobRespondsWithNoContent()
        {
            var request = new MailingListAddMember() { Email = "test@test.com", FirstName = "John", LastName = "Doe" };

            var response = _controller.AddMember(request);

            response.Should().BeOfType<NoContentResult>();
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" && 
                IsMatch(request.Candidate, (Candidate)job.Args[0])),
                It.IsAny<EnqueuedState>()));
        }

        private static bool IsMatch(Candidate candidateA, Candidate candidateB)
        {
            candidateA.Should().BeEquivalentTo(candidateB, options => options
                .Excluding(c => c.Subscriptions[0].StartAt)
                .Excluding(c => c.PrivacyPolicy.AcceptedAt));
            return true;
        }
    }
}
