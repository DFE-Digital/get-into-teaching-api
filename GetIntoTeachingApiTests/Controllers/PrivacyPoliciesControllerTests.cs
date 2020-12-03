using GetIntoTeachingApi.Controllers;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Xunit;
using GetIntoTeachingApi.Attributes;

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
        public void LogRequests_IsPresent()
        {
            typeof(PrivacyPoliciesController).Should().BeDecoratedWith<LogRequestsAttribute>();
        }

        [Fact]
        public void PrivateShortTermResponseCache_IsPresent()
        {
            typeof(PrivacyPoliciesController).Should().BeDecoratedWith<PrivateShortTermResponseCacheAttribute>();
        }

        [Fact]
        public void CrmETag_IsPresent()
        {
            JobStorage.Current = new Mock<JobStorage>().Object;
            typeof(PrivacyPoliciesController).GetMethod("GetLatest").Should().BeDecoratedWith<CrmETagAttribute>();
        }

        [Fact]
        public async void Get_ReturnsPrivacyPolicy()
        {
            var policy = new PrivacyPolicy() { Id = Guid.NewGuid() };
            _mockStore.Setup(mock => mock.GetPrivacyPolicyAsync((Guid)policy.Id)).ReturnsAsync(policy);

            var response = await _controller.Get((Guid)policy.Id);

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(policy);
        }

        [Fact]
        public async void Get_WithMissingEvent_ReturnsNotFound()
        {
            _mockStore.Setup(mock => mock.GetTeachingEventAsync(It.IsAny<Guid>())).ReturnsAsync(null as TeachingEvent);

            var response = await _controller.Get(Guid.NewGuid());

            response.Should().BeOfType<NotFoundResult>();
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
            return new PrivacyPolicy { Id = Guid.NewGuid(), Text = "Example text", CreatedAt = DateTime.UtcNow };
        }
    }
}
