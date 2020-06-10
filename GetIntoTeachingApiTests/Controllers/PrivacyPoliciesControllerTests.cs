using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Helpers;
﻿using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Controllers
{
    public class PrivacyPoliciesControllerTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly Mock<ILogger<PrivacyPoliciesController>> _mockLogger;
        private readonly PrivacyPoliciesController _controller;

        public PrivacyPoliciesControllerTests()
        {
            _mockStore = new Mock<IStore>();
            _mockLogger = new Mock<ILogger<PrivacyPoliciesController>>();
            _controller = new PrivacyPoliciesController(_mockLogger.Object, _mockStore.Object);
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
            _mockStore.Setup(mock => mock.GetLatestPrivacyPolicyAsync()).ReturnsAsync(mockPolicy);

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
