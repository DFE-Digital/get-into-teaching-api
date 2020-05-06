using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using GetIntoTeachingApiTests.Utils;

namespace GetIntoTeachingApiTests.Controllers
{
    public class TeachingEventsControllerTests
    {
        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(TeachingEventsController));
        }

        [Fact]
        public void AddAttendee_InvalidRequest_RespondsWithValidationErrors()
        {
            var attendee = new CandidateIdentification { FirstName = null };
            var mockLogger = new Mock<ILogger<TeachingEventsController>>();
            var controller = new TeachingEventsController(mockLogger.Object);
            controller.ModelState.AddModelError("FirstName", "First name must be specified.");

            var response = controller.AddAttendee("123", attendee);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("FirstName").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("First name must be specified.");
        }
    }
}
