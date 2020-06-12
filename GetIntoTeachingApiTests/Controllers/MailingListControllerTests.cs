using Xunit;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;

namespace GetIntoTeachingApiTests.Controllers
{
    public class MailingListControllerTests
    {
        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(MailingListController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void CreateCandidateAccessToken_InvalidRequest_RespondsWithValidationErrors()
        {
            var member = new ExistingCandidateRequest() { FirstName = null };
            var controller = new MailingListController();
            controller.ModelState.AddModelError("FirstName", "First name must be specified.");

            var response = controller.AddMember(member);

            var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errors = badRequest.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().ContainKey("FirstName").WhichValue.Should().BeOfType<string[]>().Which.Should().Contain("First name must be specified.");
        }
    }
}
