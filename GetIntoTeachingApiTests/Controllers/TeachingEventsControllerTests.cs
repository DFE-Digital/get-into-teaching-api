using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;

namespace GetIntoTeachingApiTests.Controllers
{
    public class TeachingEventsControllerTests
    {
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly TeachingEventsController _controller;

        public TeachingEventsControllerTests()
        {
            var mockLogger = new Mock<ILogger<TeachingEventsController>>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _controller = new TeachingEventsController(mockLogger.Object, _mockCrm.Object, _mockJobClient.Object);
        }

        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(TeachingEventsController));
        }

        [Fact]
        public void AddAttendee_InvalidRequest_RespondsWithValidationErrors()
        {
            var attendee = new ExistingCandidateRequest() { FirstName = null };
            _controller.ModelState.AddModelError("FirstName", "First name must be specified.");

            var response = _controller.AddAttendee(Guid.NewGuid(), attendee);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("FirstName").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("First name must be specified.");
        }

        [Fact]
        public void AddAttendee_MissingEvent_RespondsWithNotFound()
        {
            var attendee = new ExistingCandidateRequest() { FirstName = null };
            var teachingEventId = Guid.NewGuid();
            _mockCrm.Setup(mock => mock.GetTeachingEvent(teachingEventId)).Returns<TeachingEvent>(null);

            var response = _controller.AddAttendee(teachingEventId, attendee);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void AddAttendee_MissingCandidate_RespondsWithNotFound()
        {
            var attendee = new ExistingCandidateRequest() { Email = "test@test.com", FirstName = "John", LastName = "Doe" };
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid() };
            _mockCrm.Setup(mock => mock.GetTeachingEvent((Guid)teachingEvent.Id)).Returns(teachingEvent);
            _mockCrm.Setup(mock => mock.GetCandidate(attendee)).Returns<Candidate>(null);

            var response = _controller.AddAttendee((Guid)teachingEvent.Id, attendee);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void AddAttendee_ValidRequest_EnqueuesJobRespondsWithNoContent()
        {
            var attendee = new ExistingCandidateRequest() { Email = "test@test.com", FirstName = "John", LastName = "Doe" };
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid() };
            var candidate = new Candidate() { Id = Guid.NewGuid(), Email = "test@test.com" };
            _mockCrm.Setup(mock => mock.GetTeachingEvent((Guid)teachingEvent.Id)).Returns(teachingEvent);
            _mockCrm.Setup(mock => mock.GetCandidate(attendee)).Returns(candidate);

            var response = _controller.AddAttendee((Guid)teachingEvent.Id, attendee);

            response.Should().BeOfType<NoContentResult>();
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(TeachingEventRegistrationJob) && job.Method.Name == "Run" &&
                                  ((TeachingEventRegistration)job.Args[0]).CandidateId == candidate.Id &&
                                  ((TeachingEventRegistration)job.Args[0]).CandidateEmail == candidate.Email &&
                                  ((TeachingEventRegistration) job.Args[0]).EventId == teachingEvent.Id), 
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Search_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new TeachingEventSearchRequest() { Postcode = null};
            _controller.ModelState.AddModelError("Postcode", "Postcode must be specified.");

            var response = _controller.Search(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Postcode").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("Postcode must be specified.");
        }

        [Fact]
        public void GetUpcoming_ValidRequest_ReturnsTeachingEvents()
        {
            var request = new TeachingEventSearchRequest() {Postcode = "KY12 8FG"};
            var mockEvents = MockEvents();
            _mockCrm.Setup(mock => mock.SearchTeachingEvents(request)).Returns(mockEvents);

            var response = _controller.Search(request);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEvents);
        }

        [Fact]
        public void GetUpcoming_LimitMoreThan50_RespondsWithBadRequest()
        {
            var response = _controller.GetUpcoming(51);
            
            response.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void GetUpcoming_ReturnsUpcomingTeachingEvents()
        {
            var mockEvents = MockEvents();
            _mockCrm.Setup(mock => mock.GetUpcomingTeachingEvents(3)).Returns(mockEvents);

            var response = _controller.GetUpcoming(3);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockEvents);
        }

        [Fact]
        public void Get_ReturnsTeachingEvents()
        {
            var teachingEvent = new TeachingEvent() {Id = Guid.NewGuid()};
            _mockCrm.Setup(mock => mock.GetTeachingEvent((Guid)teachingEvent.Id)).Returns(teachingEvent);

            var response = _controller.Get((Guid)teachingEvent.Id);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(teachingEvent);
        }

        [Fact]
        public void Get_WithMissingEvent_ReturnsNotFound()
        {
            _mockCrm.Setup(mock => mock.GetTeachingEvent(It.IsAny<Guid>())).Returns<TeachingEvent>(null);

            var response = _controller.Get(Guid.NewGuid());

            response.Should().BeOfType<NotFoundResult>();
        }

        private static IEnumerable<TeachingEvent> MockEvents()
        {
            var event1 = new TeachingEvent() { Name = "Event 1" };
            var event2 = new TeachingEvent() { Name = "Event 2" };
            var event3 = new TeachingEvent() { Name = "Event 3" };

            return new[] {event1, event2, event3};
        }
    }
}
