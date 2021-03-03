using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GetIntoTeachingApi.Services;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;
using MoreLinq;
using GetIntoTeachingApi.Attributes;
using System.Linq;
using Microsoft.Extensions.Logging;
using GetIntoTeachingApiTests.Helpers;
using System.Text.Json;
using GetIntoTeachingApi.Utils;

namespace GetIntoTeachingApiTests.Controllers
{
    public class TeachingEventsControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<IStore> _mockStore;
        private readonly Mock<ILogger<TeachingEventsController>> _mockLogger;
        private readonly IMetricService _metrics;
        private readonly TeachingEventsController _controller;
        private readonly ExistingCandidateRequest _request;

        public TeachingEventsControllerTests()
        {
            _request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            _mockTokenService = new Mock<ICandidateAccessTokenService>();
            _mockCrm = new Mock<ICrmService>();
            _mockStore = new Mock<IStore>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _mockLogger = new Mock<ILogger<TeachingEventsController>>();
            _metrics = new MetricService();
            _controller = new TeachingEventsController(_mockStore.Object, _mockJobClient.Object,
                _mockTokenService.Object, _mockCrm.Object, _mockLogger.Object, _metrics);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(TeachingEventsController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,GetIntoTeaching");
        }

        [Fact]
        public void CrmETag_IsPresent()
        {
            JobStorage.Current = new Mock<JobStorage>().Object;
            var methods = new [] { "Get", "SearchGroupedByType" };

            methods.ForEach(m => typeof(TeachingEventsController).GetMethod(m).Should().BeDecoratedWith<CrmETagAttribute>());
        }

        [Fact]
        public void CrmETagPrivateShortTermResponseCache_IsPresent()
        {
            JobStorage.Current = new Mock<JobStorage>().Object;
            var methods = new[] { "Get", "SearchGroupedByType" };

            methods.ForEach(m => typeof(TeachingEventsController).GetMethod(m).Should().BeDecoratedWith<PrivateShortTermResponseCacheAttribute>());
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
                IsMatch(request.Candidate, (string)job.Args[0])),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public async void SearchGroupedByType_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new TeachingEventSearchRequest() { Postcode = null };
            _controller.ModelState.AddModelError("Postcode", "Postcode must be specified.");

            var response = await _controller.SearchGroupedByType(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Postcode").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("Postcode must be specified.");
        }

        [Fact]
        public async void SearchGroupedByType_ValidRequest_ReturnsTeachingEventsByType()
        {
            var request = new TeachingEventSearchRequest() { Postcode = "KY12 8FG" };
            var mockEvents = MockEvents();
            _mockStore.Setup(mock => mock.SearchTeachingEventsAsync(request)).ReturnsAsync(mockEvents);

            var response = await _controller.SearchGroupedByType(request);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var result = (IEnumerable<TeachingEventsByType>)ok.Value;

            result.First().TypeId.Should().Be(123);
            result.First().TeachingEvents.Count().Should().Be(3);
            result.Last().TypeId.Should().Be(456);
            result.Last().TeachingEvents.Count().Should().Be(1);

            _mockLogger.VerifyInformationWasCalled("SearchGroupedByType: KY12 8FG");

            _metrics.TeachingEventSearchResults.WithLabels(new[] { request.TypeId.ToString(), request.Radius.ToString() }).Count.Should().Be(1);
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
        public void ExchangeAccessTokenForAttendee_InvalidAccessToken_RespondsWithUnauthorized()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);
            _mockTokenService.Setup(mock => mock.IsValid("000000", _request, (Guid)candidate.Id)).Returns(false);

            var response = _controller.ExchangeAccessTokenForAttendee("000000", _request);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void ExchangeAccessTokenForAttendee_ValidToken_RespondsWithTeachingEventAddAttendee()
        {
            var candidate = new Candidate { Id = Guid.NewGuid() };
            _mockTokenService.Setup(tokenService => tokenService.IsValid("000000", _request, (Guid)candidate.Id)).Returns(true);
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns(candidate);

            var response = _controller.ExchangeAccessTokenForAttendee("000000", _request);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var responseModel = ok.Value as TeachingEventAddAttendee;
            responseModel.CandidateId.Should().Be(candidate.Id);
        }

        [Fact]
        public void ExchangeAccessTokenForAttendee_MissingCandidate_RespondsWithUnauthorized()
        {
            _mockCrm.Setup(mock => mock.MatchCandidate(_request)).Returns<Candidate>(null);

            var response = _controller.ExchangeAccessTokenForAttendee("000000", _request);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void GetTeachingEventBuildings_ReturnsAllTeachingEventBuildings()
        {
            var mockBuildings = new List<TeachingEventBuilding>()
            {
                new TeachingEventBuilding() { AddressCity = "test" }
            }
            .AsQueryable();

            _mockStore.Setup(mock => mock.GetTeachingEventBuildings()).Returns(mockBuildings);

            var response = _controller.GetTeachingEventBuildings();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockBuildings);
        }

        private static IEnumerable<TeachingEvent> MockEvents()
        {
            var event1 = new TeachingEvent() { Name = "Event 1", TypeId = 123 };
            var event2 = new TeachingEvent() { Name = "Event 2", TypeId = 123 };
            var event3 = new TeachingEvent() { Name = "Event 3", TypeId = 123 };
            var event4 = new TeachingEvent() { Name = "Event 4", TypeId = 123 };
            var event5 = new TeachingEvent() { Name = "Event 5", TypeId = 456 };

            return new[] { event1, event2, event3, event4, event5 };
        }

        private static bool IsMatch(Candidate candidateA, string candidateBJson)
        {
            var candidateB = candidateBJson.DeserializeChangedTracked<Candidate>();

            // Compares ignoring date attributes that are dynamic.
            candidateA.Should().BeEquivalentTo(candidateB, options => options
                .Excluding(c => c.EventsSubscriptionStartAt)
                .Excluding(c => c.PrivacyPolicy.AcceptedAt));
            return true;
        }
    }
}
