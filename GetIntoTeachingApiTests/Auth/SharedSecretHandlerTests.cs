﻿using System.Text.Encodings.Web;
using FluentAssertions;
using GetIntoTeachingApi.Auth;
using GetIntoTeachingApi.Utils;
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

        public SharedSecretHandlerTests()
        {
            var mockEnv = new Mock<IEnv>();
            mockEnv.Setup(m => m.SharedSecret).Returns("shared_secret");

            var mockOptionsMonitor = new Mock<IOptionsMonitor<SharedSecretSchemeOptions>>();
            mockOptionsMonitor.Setup(m => m.Get("SharedSecretHandler")).Returns(new SharedSecretSchemeOptions());

            var mockLogger = new Mock<ILogger<SharedSecretHandler>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);

            _handler = new SharedSecretHandler(mockEnv.Object, mockOptionsMonitor.Object, 
                mockLoggerFactory.Object, new Mock<UrlEncoder>().Object, new Mock<ISystemClock>().Object);
        }

        [Theory]
        [InlineData("Bearer shared_secret", true)]
        [InlineData("shared_secret", true)]
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
    }
}
