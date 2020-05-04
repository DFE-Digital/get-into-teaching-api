using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using System.Collections.Generic;

namespace GetIntoTeachingApiTests.Controllers
{
    public class CandidatesControllerTests
    {
        [Fact]
        public void CreateAccessToken_InvalidRequest_RespondsWithValidationErrors()
        {
            var request = new CandidateAccessTokenRequest { Email = "invalid-email@" };
            var mockLogger = new Mock<ILogger<CandidatesController>>();
            var controller = new CandidatesController(mockLogger.Object);
            controller.ModelState.AddModelError("Email", "Email is invalid.");

            var response = controller.CreateAccessToken(request);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("Email").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("Email is invalid.");
        }
    }
}
