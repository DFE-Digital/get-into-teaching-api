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
    public class MailingListControllerTests
    {
        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(MailingListController));
        }

        [Fact]
        public void CreateCandidateAccessToken_InvalidRequest_RespondsWithValidationErrors()
        {
            var member = new CandidateIdentification { FirstName = null };
            var mockLogger = new Mock<ILogger<MailingListController>>();
            var controller = new MailingListController(mockLogger.Object);
            controller.ModelState.AddModelError("FirstName", "First name must be specified.");

            var response = controller.AddMember(member);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("FirstName").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("First name must be specified.");
        }
    }
}
