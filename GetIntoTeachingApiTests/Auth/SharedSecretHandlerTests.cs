using System.Text.Encodings.Web;
using FluentAssertions;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Auth
{
    public class SharedSecretHandlerTests
    {
        private readonly SharedSecretHandler _handler;
        private readonly Mock<ILogger<SharedSecretHandler>> _mockLogger;

        public SharedSecretHandlerTests()
        {
            var mockEnv = new Mock<IEnv>();
            mockEnv.Setup(m => m.SharedSecret).Returns("shared_secret");
            mockEnv.Setup(m => m.PenTestSharedSecret).Returns("pen_test_shared_secret");

            var mockOptionsMonitor = new Mock<IOptionsMonitor<SharedSecretSchemeOptions>>();
            mockOptionsMonitor.Setup(m => m.Get("SharedSecretHandler")).Returns(new SharedSecretSchemeOptions());

            _mockLogger = new Mock<ILogger<SharedSecretHandler>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            _handler = new SharedSecretHandler(mockEnv.Object, mockOptionsMonitor.Object,
                mockLoggerFactory.Object, new Mock<UrlEncoder>().Object, new Mock<ISystemClock>().Object);
        }

        [Theory]
        [InlineData("Bearer shared_secret", true)]
        [InlineData("shared_secret", true)]
        [InlineData("Bearer pen_test_shared_secret", true)]
        [InlineData("pen_test_shared_secret", true)]
        [InlineData("Bearer incorrect_shared_secret", false)]
        [InlineData("Bearer ", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public async void InitializeAsync_AuthenticatesCorrectly(string authHeaderValue, bool expected)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", authHeaderValue);
            var scheme = new AuthenticationScheme("SharedSecretHandler", null, typeof(SharedSecretHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Succeeded.Should().Be(expected);
        }

        [Fact]
        public async void InitializeAsync_NoAuthorizationHeader_ReturnsNoResult()
        {
            var context = new DefaultHttpContext();
            var scheme = new AuthenticationScheme("SharedSecretHandler", null, typeof(SharedSecretHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Should().BeEquivalentTo(AuthenticateResult.NoResult());
        }

        [Fact]
        public async void InitializeAsync_IncorrectAuthorizationHeader_LogsWarning()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", "incorrect_shared_secret");
            var scheme = new AuthenticationScheme("SharedSecretHandler", null, typeof(SharedSecretHandler));
            await _handler.InitializeAsync(scheme, context);

            await _handler.AuthenticateAsync();

            _mockLogger.VerifyWarningWasCalled("SharedSecretHandler - Token is not valid");
        }
    }
}
