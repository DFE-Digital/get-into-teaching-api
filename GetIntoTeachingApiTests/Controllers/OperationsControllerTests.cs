using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Moq;
using Xunit;
using GetIntoTeachingApi.Attributes;
using System;

namespace GetIntoTeachingApiTests.Controllers
{
    public class OperationsControllerTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Mock<IHangfireService> _mockHangfire;
        private readonly Mock<IEnv> _mockEnv;
        private readonly OperationsController _controller;

        public OperationsControllerTests()
        {
            _mockStore = new Mock<IStore>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockHangfire = new Mock<IHangfireService>();
            _mockEnv = new Mock<IEnv>();

            _controller = new OperationsController(
                _mockCrm.Object,
                _mockStore.Object,
                _mockNotifyService.Object,
                _mockHangfire.Object,
                _mockEnv.Object);
        }

        [Fact]
        public void GenerateMappingInfo_RespondsWithMappingInfo()
        {
            var response = _controller.GenerateMappingInfo();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var mappings = ok.Value.Should().BeOfType<List<MappingInfo>>().Subject;
            mappings.Count().Should().Be(10);
            mappings.Any(m => m.LogicalName == "contact" &&
                              m.Class == "GetIntoTeachingApi.Models.Candidate"
            ).Should().BeTrue();
        }

        [Fact]
        public void SimulateError_ThrowsNullReferenceException()
        {
            _controller.Invoking(c => c.SimulateError()).Should().Throw<NullReferenceException>();
        }

        [Theory]
        [InlineData(true, true, true, true, "healthy")]
        [InlineData(true, true, false, true, "degraded")]
        [InlineData(true, true, false, false, "degraded")]
        [InlineData(true, true, true, false, "degraded")]
        [InlineData(false, true, true, true, "unhealthy")]
        [InlineData(true, false, true, true, "unhealthy")]
        [InlineData(false, false, true, true, "unhealthy")]
        [InlineData(true, false, false, true, "unhealthy")]
        [InlineData(false, false, false, false, "unhealthy")]
        public async void HealthCheck_ReturnsCorrectly(bool database, bool hangfire, bool crm, bool notify, string expectedStatus)
        {
            const string sha = "3c42c1051f6eb535c2017eafb660d1a884a39722";
            const string environmentName = "Test";

            var databaseStatus = database ? HealthCheckResponse.StatusOk : "error";
            var hangfireStatus = hangfire ? HealthCheckResponse.StatusOk : "error";
            var crmStatus = crm ? HealthCheckResponse.StatusOk : "error";
            var notifyStatus = notify ? HealthCheckResponse.StatusOk : "error";

            _mockEnv.Setup(m => m.GitCommitSha).Returns(sha);
            _mockEnv.Setup(m => m.EnvironmentName).Returns(environmentName);
            _mockStore.Setup(m => m.CheckStatusAsync()).ReturnsAsync(databaseStatus);
            _mockNotifyService.Setup(m => m.CheckStatusAsync()).ReturnsAsync(notifyStatus);
            _mockHangfire.Setup(m => m.CheckStatus()).Returns(hangfireStatus);
            _mockCrm.Setup(m => m.CheckStatus()).Returns(crmStatus);

            var response = await _controller.HealthCheck();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var health = (HealthCheckResponse)ok.Value;

            health.GitCommitSha.Should().Be(sha);
            health.Environment.Should().Be(environmentName);
            health.Database.Should().Be(databaseStatus);
            health.Crm.Should().Be(crmStatus);
            health.Notify.Should().Be(notifyStatus);
            health.Hangfire.Should().Be(hangfireStatus);
            health.Status.Should().Be(expectedStatus);
        }

        [Fact]
        public void LogRequests_IsPresent()
        {
            typeof(OperationsController).Should().BeDecoratedWith<LogRequestsAttribute>();
        }
    }
}