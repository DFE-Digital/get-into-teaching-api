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
        private readonly Mock<IHttpContextAccessor> _mockHttpHandler;
        private readonly AuthorizationHandlerContext _context;

        public SharedSecretHandlerTests()
        {
            _mockHttpHandler = new Mock<IHttpContextAccessor>();
            _handler = new SharedSecretHandler(_mockHttpHandler.Object);

            var requirements = new[] { new SharedSecretRequirement("shared_secret") };
            _context = new AuthorizationHandlerContext(requirements, null, null);
        }

        [Fact]
        public async void HandleRequirement_WithCorrectSecret_CallsSucceed()
        {
            _mockHttpHandler.Setup(mock => mock.HttpContext.Request.Headers["Authorization"]).Returns("Bearer shared_secret");

            await _handler.HandleAsync(_context);

            _context.HasSucceeded.Should().BeTrue();
        }

        [Fact]
        public async void HandleRequirement_WithIncorrectSecret_DoesNotCallSucceed()
        {
            _mockHttpHandler.Setup(mock => mock.HttpContext.Request.Headers["Authorization"]).Returns("Bearer incorrect_shared_secret");

            await _handler.HandleAsync(_context);

            _context.HasSucceeded.Should().BeFalse();
        }

        [Fact]
        public async void HandleRequirement_WithNoSecret_DoesNotCallSucceed()
        {
            _mockHttpHandler.Setup(mock => mock.HttpContext.Request.Headers["Authorization"]).Returns<string>(null);

            await _handler.HandleAsync(_context);

            _context.HasSucceeded.Should().BeFalse();
        }
    }
}
