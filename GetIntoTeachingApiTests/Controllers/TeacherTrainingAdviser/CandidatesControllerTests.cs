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
            var challenge = new CandidateAccessTokenChallenge { Token = "000000", Email = "email@address.com" };
            mockTokenService.Setup(tokenService => tokenService.IsValid(challenge)).Returns(false);
            var mockLogger = new Mock<ILogger<CandidatesController>>();
            var controller = new CandidatesController(mockLogger.Object, mockTokenService.Object);

            var response = controller.Get("000000", "email@address.com");

            response.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
