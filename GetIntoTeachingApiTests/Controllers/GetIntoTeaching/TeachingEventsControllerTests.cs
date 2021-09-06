using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
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
using GetIntoTeachingApi.Utils;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Controllers.GetIntoTeaching;

namespace GetIntoTeachingApiTests.Controllers.GetIntoTeaching
{
    public class TeachingEventsControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<IStore> _mockStore;
        private readonly Mock<ILogger<TeachingEventsController>> _mockLogger;
        private readonly Mock<IDateTimeProvider> _mockDateTime;
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
            _mockDateTime = new Mock<IDateTimeProvider>();
            _metrics = new MetricService();
            _controller = new TeachingEventsController(_mockStore.Object, _mockJobClient.Object,
                _mockTokenService.Object, _mockCrm.Object, _mockLogger.Object, _metrics, _mockDateTime.Object);

            // Freeze time.
            _mockDateTime.Setup(m => m.UtcNow).Returns(DateTime.UtcNow);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(TeachingEventsController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,GetIntoTeaching");
        }

        [Fact]
        public void PrivateShortTermResponseCache_IsPresent()
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
            errors.Should().ContainKey("FirstName").WhoseValue.Should().BeOfType<string[]>().Which.Should().Contain("First name must be specified.");
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
        public void SearchGroupedByType_SearchRequestIsDecoratedWithCommaSeparatedAttributeForArrayProperties()
        {
            var expectedAttribute = new CommaSeparatedAttribute(
                nameof(TeachingEventSearchRequest.TypeIds), nameof(TeachingEventSearchRequest.StatusIds));

            typeof(TeachingEventsController)
                .GetMethod("SearchGroupedByType")
                .GetParameters()
                .FirstOrDefault(param => param.ParameterType == typeof(TeachingEventSearchRequest))
                .GetCustomAttributes(false)
                .Should().ContainEquivalentOf(expectedAttribute);
        }

        [Fact]
        public async void SearchGroupedByType_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new TeachingEventSearchRequest() { Postcode = null };
            _controller.ModelState.AddModelError("Postcode", "Postcode must be specified.");

            var response = await _controller.SearchGroupedByType(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Postcode").WhoseValue.Should().BeOfType<string[]>().Which.Should().Contain("Postcode must be specified.");
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

            _metrics.TeachingEventSearchResults.WithLabels(new[] { string.Empty, request.Radius.ToString() }).Count.Should().Be(1);
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
        public async Task Upsert_WhenRequestIsInvalid_RepondsWithValidationErrorAsync()
        {
            const string expectedErrorKey = "Name";
            const string expectedErrorMessage = "Name must be specified";
            var request = new TeachingEvent();
            _controller.ModelState.AddModelError(expectedErrorKey, expectedErrorMessage);
            var apiBehaviorOptions = new ApiBehaviorOptions
            {
                InvalidModelStateResponseFactory = (context) => new BadRequestObjectResult(_controller.ModelState)
            };

            var response = await _controller.Upsert(request, Options.Create(apiBehaviorOptions));

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey(expectedErrorKey).WhoseValue.Should().BeOfType<string[]>().Which.Should().Contain(expectedErrorMessage);
        }

        [Fact]
        public async Task Upsert_WhenReadableIdIsNotUnique_RepondsWithValidationErrorAsync()
        {
            var existingTeachingEvent = new TeachingEvent() { Id = Guid.NewGuid(), ReadableId = "existing" };
            _mockCrm.Setup(m => m.GetTeachingEvent("existing")).Returns(existingTeachingEvent);
            var request = new TeachingEvent() { ReadableId = "existing" };
            var apiBehaviorOptions = new ApiBehaviorOptions
            {
                InvalidModelStateResponseFactory = (context) => new BadRequestObjectResult(_controller.ModelState)
            };

            var response = await _controller.Upsert(request, Options.Create(apiBehaviorOptions));

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("ReadableId").WhoseValue.Should().BeOfType<string[]>().Which.Should().Contain("Must be unique");
        }

        [Fact]
        public async Task Upsert_ValidRequestWithoutBuilding_SavesInCrmAndCaches()
        {
            const string testName = "test";
            var newTeachingEvent = new TeachingEvent() { Name = testName };
            _mockCrm.Setup(mock => mock.Save(newTeachingEvent)).Verifiable();
            _mockStore.Setup(mock => mock.SaveAsync(newTeachingEvent)).Verifiable();

            var response = await _controller.Upsert(newTeachingEvent, null);

            _mockCrm.Verify();
            _mockStore.Verify();
            var created = response.Should().BeOfType<CreatedAtActionResult>().Subject;
            var teachingEvent = created.Value.Should().BeAssignableTo<TeachingEvent>().Subject;
            teachingEvent.Name.Should().Be(testName);
        }

        [Fact]
        public async Task Upsert_ValidRequestWithBuilding_SavesInCrmAndCaches()
        {
            const string testName = "test";
            var buildingId = Guid.NewGuid();
            var newBuilding = new TeachingEventBuilding() { Id = buildingId };
            var newTeachingEvent = new TeachingEvent() { Name = testName, Building = newBuilding };
            _mockCrm.Setup(mock => mock.Save(newBuilding)).Verifiable();
            _mockCrm.Setup(mock => mock.Save(It.Is<TeachingEvent>(e => e.BuildingId == buildingId))).Verifiable();
            _mockStore.Setup(mock => mock.SaveAsync(newBuilding)).Verifiable();
            _mockStore.Setup(mock => mock.SaveAsync(newTeachingEvent)).Verifiable();

            var response = await _controller.Upsert(newTeachingEvent, null);

            _mockCrm.Verify();
            _mockStore.Verify();
            var created = response.Should().BeOfType<CreatedAtActionResult>().Subject;
            var teachingEvent = created.Value.Should().BeAssignableTo<TeachingEvent>().Subject;
            teachingEvent.Name.Should().Be(testName);
            teachingEvent.Building.Should().Be(newBuilding);
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
            var candidateB = candidateBJson.DeserializeChangeTracked<Candidate>();
            candidateA.Should().BeEquivalentTo(candidateB);
            return true;
        }
    }
}
