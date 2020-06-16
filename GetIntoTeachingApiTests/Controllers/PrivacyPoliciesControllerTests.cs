using GetIntoTeachingApi.Controllers;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace GetIntoTeachingApiTests.Controllers
{
    public class PrivacyPoliciesControllerTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly PrivacyPoliciesController _controller;

        public PrivacyPoliciesControllerTests()
        {
            _mockStore = new Mock<IStore>();
            _controller = new PrivacyPoliciesController(_mockStore.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(PrivacyPoliciesController).Should().BeDecoratedWith<AuthorizeAttribute>();
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
