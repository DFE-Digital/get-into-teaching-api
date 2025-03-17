﻿using FluentAssertions;
using GetIntoTeachingApi.Controllers.GetIntoTeaching;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using GetIntoTeachingApiTests.Models.GetIntoTeaching.TestDoubles;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Xunit;
using CurrentYearProviderTestDouble = GetIntoTeachingApiTests.Models.GetIntoTeaching.TestDoubles.CurrentYearProviderTestDouble;

namespace GetIntoTeachingApiTests.Controllers.GetIntoTeaching
{
    public class MailingListControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockAccessTokenService;
        private readonly Mock<ICandidateMagicLinkTokenService> _mockMagicLinkTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<IDateTimeProvider> _mockDateTime;
        private readonly MailingListController _controller;
        private readonly ExistingCandidateRequest _request;

        public MailingListControllerTests()
        {
            _request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockAccessTokenService = new Mock<ICandidateAccessTokenService>();
            _mockMagicLinkTokenService = new Mock<ICandidateMagicLinkTokenService>();
            _mockDateTime = new Mock<IDateTimeProvider>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _controller = new MailingListController(
                _mockAccessTokenService.Object,
                _mockMagicLinkTokenService.Object,
                _mockCrm.Object,
                _mockJobClient.Object,
                _mockDateTime.Object,
                DegreeStatusDomainServiceTestDouble.DefaultMockObject(),
                CurrentYearProviderTestDouble.DefaultMockObject());
            _controller.MockUser("GIT");

            // Freeze time.
            _mockDateTime.Setup(m => m.UtcNow).Returns(DateTime.UtcNow);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(MailingListController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,GetIntoTeaching");
        }

        [Fact]
        public void ExchangeAccessTokenForMember_MissingCandidate_RespondsWithUnauthorized()
        {
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns<Candidate>(null);

            var response = _controller.ExchangeAccessTokenForMember("000000", _request);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void ExchangeAccessTokenForMember_InvalidAccessToken_RespondsWithUnauthorized()
        {
            _request.Reference = "Ref";
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);
            _mockAccessTokenService.Setup(mock => mock.IsValid("000000", _request, (Guid)candidate.Id)).Returns(false);

            var response = _controller.ExchangeAccessTokenForMember("000000", _request);

            _request.Reference.Should().Be("Ref");
            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void ExchangeAccessTokenForMember_ValidToken_RespondsWithMailingListAddMember()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockAccessTokenService.Setup(tokenService => tokenService.IsValid("000000", _request, (Guid)candidate.Id)).Returns(true);
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);

            var response = _controller.ExchangeAccessTokenForMember("000000", _request);

            _request.Reference.Should().Be("GIT");
            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var responseModel = ok.Value as MailingListAddMember;
            responseModel.CandidateId.Should().Be(candidate.Id);
        }

        [Fact]
        public void ExchangeMagicLinkTokenForMember_ValidToken_RespondsWithMailingListAddMember()
        {
            var candidate = new Candidate { Id = Guid.NewGuid(), MagicLinkTokenExpiresAt = DateTime.UtcNow.AddMinutes(1) };
            var result = new CandidateMagicLinkExchangeResult(candidate);
            _mockMagicLinkTokenService.Setup(m => m.Exchange(candidate.MagicLinkToken)).Returns(result);

            var response = _controller.ExchangeMagicLinkTokenForMember(candidate.MagicLinkToken);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var responseModel = ok.Value as MailingListAddMember;
            responseModel.CandidateId.Should().Be(candidate.Id);
        }

        [Fact]
        public void ExchangeMagicLinkTokenForMember_ValidToken_UpdatesTokenAsExchanged()
        {
            var candidate = new Candidate { Id = Guid.NewGuid(), MagicLinkTokenExpiresAt = DateTime.UtcNow.AddMinutes(1) };
            var result = new CandidateMagicLinkExchangeResult(candidate);
            _mockMagicLinkTokenService.Setup(m => m.Exchange(candidate.MagicLinkToken)).Returns(result);

            var response = _controller.ExchangeMagicLinkTokenForMember(candidate.MagicLinkToken);

            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                IsMatch(candidate, (string)job.Args[0])),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void ExchangeMagicLinkTokenForMember_InvalidToken_RespondsWithUnauthorized()
        {
            var token = Guid.NewGuid().ToString();
            var result = new CandidateMagicLinkExchangeResult(null);
            _mockMagicLinkTokenService.Setup(m => m.Exchange(token)).Returns(result);

            var response = _controller.ExchangeMagicLinkTokenForMember(token);

            var unauthorized = response.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorized.Value.Should().BeEquivalentTo(result);
        }

        [Fact]
        public void AddMember_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new MailingListAddMember() { FirstName = null };
            _controller.ModelState.AddModelError("FirstName", "First name must be specified.");

            var response = _controller.AddMember(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("FirstName").WhoseValue.Should().BeOfType<string[]>().Which.Should().Contain("First name must be specified.");
        }

        [Fact]
        public void AddMember_ValidRequest_EnqueuesJobRespondsWithNoContent()
        {
            var request = new MailingListAddMember() { Email = "test@test.com", FirstName = "John", LastName = "Doe" };

            var response = _controller.AddMember(request);

            response.Should().BeOfType<NoContentResult>();
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                IsMatch(request.Candidate, (string)job.Args[0])),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void AddMember_ValidRequestWithCandidateSituation_EnqueuesJobRespondsWithNoContent()
        {
            var request = new MailingListAddMember() { Email = "test@test.com", FirstName = "John", LastName = "Doe", Situation = 123456 };

            var response = _controller.AddMember(request);

            response.Should().BeOfType<NoContentResult>();
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                IsMatch(request.Candidate, (string)job.Args[0])),
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
