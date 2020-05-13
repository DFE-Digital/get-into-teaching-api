using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Controllers.TeacherTrainingAdviser;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApiTests.Utils;

namespace GetIntoTeachingApiTests.TeacherTrainingAdvisor.Controllers
{
    public class CandidatesControllerTests
    {
        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(CandidatesController));
        }

        [Fact]
        public void Get_InvalidAccessToken_RespondsWithUnauthorized()
        {
            var mockTokenService = new Mock<ICandidateAccessTokenService>();
            var mockCrm = new Mock<ICrmService>();
            var request = new ExistingCandidateRequest { Email = "email@address.com", FirstName = "John", LastName = "Doe" };
            mockTokenService.Setup(tokenService => tokenService.IsValid("000000", request)).Returns(false);
            var mockLogger = new Mock<ILogger<CandidatesController>>();
            var controller = new CandidatesController(mockLogger.Object, mockTokenService.Object, mockCrm.Object);

            var response = controller.Get("000000", request);

            response.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
