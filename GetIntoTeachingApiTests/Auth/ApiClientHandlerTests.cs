using System.Security.Claims;
using System.Text.Encodings.Web;
using FluentAssertions;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
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
        [InlineData("Bearer api_key", true)]
        [InlineData("api_key", true)]
        [InlineData("Bearer incorect_api_key", false)]
        [InlineData("Bearer api_key ", false)]
        [InlineData("Bearer ", false)]
        [InlineData("api key", false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        public async void InitializeAsync_WithApiClient_AuthenticatesCorrectly(string authHeaderValue, bool expected)
        {
            var client = new Client() { Name = "Admin", Description = "Admin account", Role = "Service", ApiKey = "api_key", ApiKeyPrefix = "ADMIN" };
            _mockClientManager.Setup(m => m.GetClient(client.ApiKey)).Returns(client);
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", authHeaderValue);
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Succeeded.Should().Be(expected);

            if (result.Succeeded)
            {
                result.Principal.HasClaim("token", "api_key").Should().BeTrue();
                result.Principal.HasClaim(ClaimTypes.Role, "Service").Should().BeTrue();
            }
        }

        [Theory]
        [InlineData("Bearer ", "")]
        [InlineData("Bearer ", null)]
        [InlineData("Bearer ", " ")]
        [InlineData("Bearer  ", " ")]
        [InlineData("Bearer", "")]
        [InlineData("Bearer", null)]
        [InlineData("Bearer", " ")]
        [InlineData("", "")]
        [InlineData("", null)]
        [InlineData(" ", " ")]
        [InlineData(" ", "")]
        [InlineData(" ", null)]
        public async void InitializeAsync_EmptyOrNullHeaderAndApiKey_ReturnsUnauthorized(string authHeaderValue, string apiKey)
        {
            var client = new Client() { Name = "Admin", Description = "Admin account", Role = "Admin", ApiKey = apiKey, ApiKeyPrefix = "ADMIN" };
            _mockClientManager.Setup(m => m.GetClient(client.ApiKey)).Returns(client);

            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", authHeaderValue);
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Succeeded.Should().BeFalse();
            result.Failure.Message.Should().Be("API key is not valid");
        }

        [Fact]
        public async void InitializeAsync_NoAuthorizationHeader_ReturnsUnauthorized()
        {
            var context = new DefaultHttpContext();
            var scheme = new AuthenticationScheme("ApiClientHandler", null, typeof(ApiClientHandler));
            await _handler.InitializeAsync(scheme, context);

            var result = await _handler.AuthenticateAsync();

            result.Succeeded.Should().BeFalse();
        }
    }
}