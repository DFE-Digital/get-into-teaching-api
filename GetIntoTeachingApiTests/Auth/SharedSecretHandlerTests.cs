using FluentAssertions;
using GetIntoTeachingApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Auth
{
    public class SharedSecretHandlerTests
    {
        private readonly SharedSecretHandler _handler;

        public SharedSecretHandlerTests()
        {
            var mockHttpHandler = new Mock<IHttpContextAccessor>();
            mockHttpHandler.Setup(mock => mock.HttpContext.Request.Headers["Authorization"]).Returns("Bearer shared_secret");
            _handler = new SharedSecretHandler(mockHttpHandler.Object);
        }

        [Fact]
        public async void HandleRequirement_WithCorrectSecret_CallsSucceed()
        {
            var requirements = new[] { new SharedSecretRequirement("shared_secret") };
            var context = new AuthorizationHandlerContext(requirements, null, null);

            await _handler.HandleAsync(context);

            context.HasSucceeded.Should().BeTrue();
        }

        [Fact]
        public async void HandleRequirement_WithIncorrectSecret_DoesNotCallSucceed()
        {
            var requirements = new[] { new SharedSecretRequirement("incorrect_shared_secret") };
            var context = new AuthorizationHandlerContext(requirements, null, null);

            await _handler.HandleAsync(context);

            context.HasSucceeded.Should().BeFalse();
        }
    }
}
