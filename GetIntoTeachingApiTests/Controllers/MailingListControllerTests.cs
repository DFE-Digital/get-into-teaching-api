using System;
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

namespace GetIntoTeachingApiTests.Controllers
{
    public class MailingListControllerTests
    {
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly MailingListController _controller;

        public MailingListControllerTests()
        {
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _controller = new MailingListController(_mockJobClient.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(MailingListController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void AddMember_InvalidRequest_RespondsWithValidationErrors()
        {
            var candidate = new Candidate() { FirstName = null };
            _controller.ModelState.AddModelError("FirstName", "First name must be specified.");

            var response = _controller.AddMember(candidate);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("FirstName").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("First name must be specified.");
        }

        [Fact]
        public void AddMember_ValidRequest_EnqueuesJobRespondsWithNoContent()
        {
            var candidate = new Candidate() { Email = "test@test.com", FirstName = "John", LastName = "Doe" };

            var response = _controller.AddMember(candidate);

            response.Should().BeOfType<NoContentResult>();
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                                  ((Candidate)job.Args[0]) == candidate),
                It.IsAny<EnqueuedState>()));
        }
    }
}
