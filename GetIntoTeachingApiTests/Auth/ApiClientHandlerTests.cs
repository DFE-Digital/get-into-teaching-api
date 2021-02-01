using System.Security.Claims;
using System.Text.Encodings.Web;
using FluentAssertions;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Auth
{
    public class ApiClientHandlerTests
    {
        private readonly ApiClientHandler _handler;
        private readonly Mock<ILogger<ApiClientHandler>> _mockLogger;
        private readonly Mock<IClientManager> _mockClientManager;

        public ApiClientHandlerTests()
        {
            _mockClientManager = new Mock<IClientManager>();

            var mockOptionsMonitor = new Mock<IOptionsMonitor<ApiClientSchemaOptions>>();
            mockOptionsMonitor.Setup(m => m.Get("ApiClientHandler")).Returns(new ApiClientSchemaOptions());

            _mockLogger = new Mock<ILogger<ApiClientHandler>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            _handler = new ApiClientHandler(_mockClientManager.Object, mockOptionsMonitor.Object,
                mockLoggerFactory.Object, new Mock<UrlEncoder>().Object, new Mock<ISystemClock>().Object);
        }

        [Theory]
        [InlineData("Bearer admin_secret", true)]
        [InlineData("admin_secret", true)]
        [InlineData("Bearer incorrect_admin_secret", false)]
        [InlineData("Bearer admin_secret ", false)]
        [InlineData("Bearer ", false)]
        [InlineData("admin secret", false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        public async void InitializeAsync_AuthenticatesCorrectly(string authHeaderValue, bool expected)
        {
            var client = new Client() { Name = "Admin", Description = "Admin account", Role = "Admin", ApiKey = "admin_secret", ApiKeyPrefix = "ADMIN" };
            _mockClientManager.Setup(m => m.GetClient(client.ApiKey)).Returns(client);
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", authHeaderValue);
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Succeeded.Should().Be(expected);

            if (result.Succeeded)
            {
                result.Principal.HasClaim("token", "admin_secret").Should().BeTrue();
                result.Principal.HasClaim(ClaimTypes.Role, "Admin").Should().BeTrue();
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async void InitializeAsync_WhenApiKeyIsNotSet_ReturnsNoResult(string apiKey)
        {
            var client = new Client() { Name = "Admin", Description = "Admin account", Role = "Admin", ApiKey = apiKey, ApiKeyPrefix = "ADMIN" };
            _mockClientManager.Setup(m => m.GetClient(client.ApiKey)).Returns(client);
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", $"Bearer {apiKey}");
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async void InitializeAsync_NoAuthorizationHeader_ReturnsNoResult()
        {
            var context = new DefaultHttpContext();
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Should().BeEquivalentTo(AuthenticateResult.NoResult());
        }

        [Fact]
        public async void InitializeAsync_IncorrectAuthorizationHeader_LogsWarning()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", "incorrect_admin_secret");
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            await _handler.AuthenticateAsync();

            _mockLogger.VerifyWarningWasCalled("ApiClientHandler - Token is not valid");
        }
    }
}