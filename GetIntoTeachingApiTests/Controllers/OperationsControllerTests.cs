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
using System;
using Microsoft.AspNetCore.Authorization;
using Hangfire;
using Hangfire.Common;
using GetIntoTeachingApi.Jobs;
using Hangfire.States;

namespace GetIntoTeachingApiTests.Controllers
{
    public class OperationsControllerTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Mock<IHangfireService> _mockHangfire;
        private readonly Mock<IRedisService> _mockRedis;
        private readonly Mock<IEnv> _mockEnv;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly OperationsController _controller;

        public OperationsControllerTests()
        {
            _mockStore = new Mock<IStore>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockHangfire = new Mock<IHangfireService>();
            _mockRedis = new Mock<IRedisService>();
            _mockEnv = new Mock<IEnv>();
            _mockAppSettings = new Mock<IAppSettings>();
            _mockJobClient = new Mock<IBackgroundJobClient>();

            _controller = new OperationsController(
                _mockCrm.Object,
                _mockStore.Object,
                _mockNotifyService.Object,
                _mockHangfire.Object,
                _mockRedis.Object,
                _mockEnv.Object,
                _mockAppSettings.Object,
                _mockJobClient.Object);
        }

        [Fact]
        public void GenerateMappingInfo_RespondsWithMappingInfo()
        {
            var response = _controller.GenerateMappingInfo();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var mappings = ok.Value.Should().BeOfType<List<MappingInfo>>().Subject;
            mappings.Count.Should().Be(14);
            mappings.Any(m => m.LogicalName == "contact" &&
                              m.Class == "GetIntoTeachingApi.Models.Crm.Candidate"
            ).Should().BeTrue();
        }

        [Fact]
        public void PauseCrmIntegration_Authorize_IsPresent()
        {
            typeof(OperationsController).GetMethod("PauseCrmIntegration")
                .Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,Crm");
        }

        [Fact]
        public void PauseCrmIntegration_PausesFor6HoursAndRespondsWithNoContent()
        {
            var sixHoursFromNow = DateTime.UtcNow.AddHours(6);

            var response = _controller.PauseCrmIntegration();

            response.Should().BeOfType<NoContentResult>();

            _mockAppSettings.VerifySet(m => m.CrmIntegrationPausedUntil =
                It.Is<DateTime>(d => VerifyDateIsCloseTo(d, sixHoursFromNow)), Times.Once());
        }

        [Fact]
        public void ResumeCrmIntegration_Authorize_IsPresent()
        {
            typeof(OperationsController).GetMethod("ResumeCrmIntegration")
                .Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin,Crm");
        }

        [Fact]
        public void ResumeCrmIntegration_ResumesAndRespondsWithNoContent()
        {
            var response = _controller.ResumeCrmIntegration();

            response.Should().BeOfType<NoContentResult>();

            _mockAppSettings.VerifySet(m => m.CrmIntegrationPausedUntil = null, Times.Once());
        }

        [Fact]
        public void BackfillFindApplyCandidates_Authorize_IsPresent()
        {
            typeof(OperationsController).GetMethod("BackfillFindApplyCandidates")
                .Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Roles == "Admin");
        }

        [Fact]
        public void BackfillFindApplyCandidates_WhenNotAlreadyRunning_EnqueuesJob()
        {
            _mockAppSettings.Setup(m => m.IsFindApplyBackfillInProgress).Returns(false);

            var response = _controller.BackfillFindApplyCandidates();

            response.Should().BeOfType<NoContentResult>();

            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(FindApplyBackfillJob) && job.Method.Name == "RunAsync"),
                It.IsAny<EnqueuedState>()), Times.Once);
        }

        [Fact]
        public void BackfillFindApplyCandidates_WhenAlreadyRunning_ReturnsBadRequest()
        {
            _mockAppSettings.Setup(m => m.IsFindApplyBackfillInProgress).Returns(true);

            var response = _controller.BackfillFindApplyCandidates();

            response.Should().BeOfType<BadRequestObjectResult>();

            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(FindApplyBackfillJob) && job.Method.Name == "RunAsync"),
                It.IsAny<EnqueuedState>()),Times.Never);
        }

        [Theory]
        [InlineData(true, true, true, true, true, "healthy")]
        [InlineData(true, true, true, false, true, "degraded")]
        [InlineData(true, true, true, false, false, "degraded")]
        [InlineData(true, true, true, true, false, "degraded")]
        [InlineData(true, true, false, true, true, "degraded")]
        [InlineData(false, true, true, true, true, "unhealthy")]
        [InlineData(true, false, true, true, true, "unhealthy")]
        [InlineData(false, false, false, true, true, "unhealthy")]
        [InlineData(true, false, false, false, true, "unhealthy")]
        [InlineData(false, false, false, false, false, "unhealthy")]
        public async void HealthCheck_ReturnsCorrectly(bool database, bool hangfire, bool redis, bool crm, bool notify, string expectedStatus)
        {
            const string sha = "3c42c1051f6eb535c2017eafb660d1a884a39722";
            const string environmentName = "Test";

            var databaseStatus = database ? HealthCheckResponse.StatusOk : "error";
            var hangfireStatus = hangfire ? HealthCheckResponse.StatusOk : "error";
            var crmStatus = crm ? HealthCheckResponse.StatusOk : "error";
            var notifyStatus = notify ? HealthCheckResponse.StatusOk : "error";
            var redisStatus = redis ? HealthCheckResponse.StatusOk : "error";

            _mockEnv.Setup(m => m.GitCommitSha).Returns(sha);
            _mockEnv.Setup(m => m.EnvironmentName).Returns(environmentName);
            _mockStore.Setup(m => m.CheckStatusAsync()).ReturnsAsync(databaseStatus);
            _mockNotifyService.Setup(m => m.CheckStatusAsync()).ReturnsAsync(notifyStatus);
            _mockRedis.Setup(m => m.CheckStatusAsync()).ReturnsAsync(redisStatus);
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
            health.Redis.Should().Be(redisStatus);
            health.Hangfire.Should().Be(hangfireStatus);
            health.Status.Should().Be(expectedStatus);
        }

        private static bool VerifyDateIsCloseTo(DateTime date, DateTime closeToDate)
        {
            date.Should().BeCloseTo(closeToDate, TimeSpan.FromSeconds(30));

            return true;
        }
    }
}