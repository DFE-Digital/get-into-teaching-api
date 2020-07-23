using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Filters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;
using MoreLinq;

namespace GetIntoTeachingApiTests.Controllers
{
    public class TeachingEventsControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<IStore> _mockStore;
        private readonly TeachingEventsController _controller;
        private readonly ExistingCandidateRequest _request;

        public TeachingEventsControllerTests()
        {
            _request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockTokenService = new Mock<ICandidateAccessTokenService>();
            _mockCrm = new Mock<ICrmService>();
            _mockStore = new Mock<IStore>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _controller = new TeachingEventsController(_mockStore.Object, _mockJobClient.Object, _mockTokenService.Object, _mockCrm.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(TeachingEventsController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void CrmETag_IsPresent()
        {
            JobStorage.Current = new Mock<JobStorage>().Object;
            var methods = new [] { "GetUpcoming", "Get", "Search" };

            methods.ForEach(m => typeof(TeachingEventsController).GetMethod(m).Should().BeDecoratedWith<CrmETagAttribute>());
        }

        [Fact]
        public void AddAttendee_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new TeachingEventAddAttendee() { EventId = Guid.NewGuid(), FirstName = null };
            _controller.ModelState.AddModelError("FirstName", "First name must be specified.");

            var response = _controller.AddAttendee(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("FirstName").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("First name must be specified.");
        }

        [Fact]
        public void AddAttendee_ValidRequest_EnqueuesJobRespondsWithNoContent()
        {
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid() };
            var request = new TeachingEventAddAttendee() { EventId = (Guid)teachingEvent.Id, Email = "test@test.com", FirstName = "John", LastName = "Doe" };

            var response = _controller.AddAttendee(request);

            response.Should().BeOfType<NoContentResult>();
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                IsMatch(request.Candidate, (Candidate)job.Args[0])),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public async void Search_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new TeachingEventSearchRequest() { Postcode = null };
            _controller.ModelState.AddModelError("Postcode", "Postcode must be specified.");

            var response = await _controller.Search(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Postcode").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("Postcode must be specified.");
        }

        [Fact]
        public async void GetUpcoming_ValidRequest_ReturnsTeachingEvents()
        {
            var request = new TeachingEventSearchRequest() { Postcode = "KY12 8FG" };
            var mockEvents = MockEvents();
            _mockStore.Setup(mock => mock.SearchTeachingEventsAsync(request)).ReturnsAsync(mockEvents);

            var response = await _controller.Search(request);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEvents);
        }

        [Fact]
        public async void GetUpcoming_LimitMoreThan50_RespondsWithBadRequest()
        {
            var response = await _controller.GetUpcoming(51);

            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async void GetUpcoming_ReturnsUpcomingTeachingEvents()
        {
            var mockEvents = MockEvents();
            _mockStore.Setup(mock => mock.GetUpcomingTeachingEvents(3)).Returns(mockEvents.AsAsyncQueryable());

            var response = await _controller.GetUpcoming(3);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockEvents);
        }

        [Fact]
        public async void Get_ReturnsTeachingEvent()
        {
            var teachingEvent = new TeachingEvent() { ReadableId = "123" };
            _mockStore.Setup(mock => mock.GetTeachingEventAsync(teachingEvent.ReadableId)).ReturnsAsync(teachingEvent);

            var response = await _controller.Get(teachingEvent.ReadableId);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(teachingEvent);
        }

        [Fact]
        public async void Get_WithMissingEvent_ReturnsNotFound()
        {
            _mockStore.Setup(mock => mock.GetTeachingEventAsync(It.IsAny<Guid>())).ReturnsAsync(null as TeachingEvent);

            var response = await _controller.Get("-1");

            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void GetAttendee_InvalidAccessToken_RespondsWithUnauthorized()
        {
            _mockTokenService.Setup(mock => mock.IsValid("000000", _request)).Returns(false);

            var response = _controller.GetAttendee("000000", _request);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void GetAttendee_ValidToken_RespondsWithTeachingEventAddAttendee()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request)).Returns(true);
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);

            var response = _controller.GetAttendee("000000", _request);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var responseModel = ok.Value as TeachingEventAddAttendee;
            responseModel.CandidateId.Should().Be(candidate.Id);
        }

        [Fact]
        public void GetAttendee_MissingCandidate_RespondsWithNotFound()
        {
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request)).Returns(true);
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns<Candidate>(null);

            var response = _controller.GetAttendee("000000", _request);

            response.Should().BeOfType<NotFoundResult>();
        }

        private static IEnumerable<TeachingEvent> MockEvents()
        {
            var event1 = new TeachingEvent() { Name = "Event 1" };
            var event2 = new TeachingEvent() { Name = "Event 2" };
            var event3 = new TeachingEvent() { Name = "Event 3" };

            return new[] { event1, event2, event3 };
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
