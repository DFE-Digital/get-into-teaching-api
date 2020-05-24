using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Utils;
﻿using FluentAssertions;
using GetIntoTeachingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using GetIntoTeachingApi.Services.Crm;
using Xunit;

namespace GetIntoTeachingApiTests.Controllers
{
    public class PrivacyPoliciesControllerTests
    {
        private readonly Mock<IWebApiClient> _mockClient;
        private readonly Mock<ILogger<PrivacyPoliciesController>> _mockLogger;
        private readonly PrivacyPoliciesController _controller;

        public PrivacyPoliciesControllerTests()
        {
            _mockClient = new Mock<IWebApiClient>();
            _mockLogger = new Mock<ILogger<PrivacyPoliciesController>>();
            _controller = new PrivacyPoliciesController(_mockLogger.Object, _mockClient.Object);
        }

        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(PrivacyPoliciesController));
        }

        [Fact]
        public async void GetLatest_ReturnsLatestPrivacyPolicy()
        {
            var mockPolicy = MockPrivacyPolicy();
            _mockClient.Setup(mock => mock.GetLatestPrivacyPolicy()).ReturnsAsync(mockPolicy);

            var response = await _controller.GetLatest();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockPolicy);
        }

        private static PrivacyPolicy MockPrivacyPolicy()
        {
            return new PrivacyPolicy { Id = Guid.NewGuid(), Text = "Example text" };
        }
    }
}
