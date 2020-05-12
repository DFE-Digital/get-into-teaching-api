using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Utils;
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
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<ILogger<PrivacyPoliciesController>> _mockLogger;
        private readonly PrivacyPoliciesController _controller;

        public PrivacyPoliciesControllerTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _mockLogger = new Mock<ILogger<PrivacyPoliciesController>>();
            _controller = new PrivacyPoliciesController(_mockLogger.Object, _mockCrm.Object);
        }

        [Fact]
        public void Authorize_HasSharedSecretPolicy()
        {
            PolicyTestHelpers.VerifyTypeIsAuthorizeWithSharedSecret(typeof(PrivacyPoliciesController));
        }

        [Fact]
        public void GetLatest_ReturnsLatestPrivacyPolicy()
        {
            PrivacyPolicy mockPolicy = MockPrivacyPolicy();
            _mockCrm.Setup(mock => mock.GetLatestPrivacyPolicy()).Returns(mockPolicy);

            var response = _controller.GetLatest();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(mockPolicy);
        }

        private PrivacyPolicy MockPrivacyPolicy()
        {
            return new PrivacyPolicy { Id = Guid.NewGuid(), Text = "Example text" };
        }
    }
}
