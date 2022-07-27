using System;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class ClaimCallbackBookingSlotJobTests
    {
        private readonly Mock<IPerformContextAdapter> _mockContext;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly DateTime _scheduledAt;
        private readonly IMetricService _metrics;
        private readonly ClaimCallbackBookingSlotJob _job;
        private readonly Mock<ILogger<ClaimCallbackBookingSlotJob>> _mockLogger;

        public ClaimCallbackBookingSlotJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockLogger = new Mock<ILogger<ClaimCallbackBookingSlotJob>>();
            _mockAppSettings = new Mock<IAppSettings>();
            _mockCrm = new Mock<ICrmService>();
            _metrics = new MetricService();
            _scheduledAt = new DateTime(2001, 1, 1, 10, 20, 30);
            _job = new ClaimCallbackBookingSlotJob(
                new Env(), new Mock<IRedisService>().Object, _mockContext.Object, _mockCrm.Object,
                _metrics, _mockLogger.Object, _mockAppSettings.Object);

            _metrics.HangfireJobQueueDuration.RemoveLabelled(new[] { "ClaimCallbackBookingSlotJob" });
            _mockContext.Setup(m => m.GetJobCreatedAt(null)).Returns(DateTime.UtcNow.AddDays(-1));

            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
        }

        [Fact]
        public void Run_OnSuccess_IncrementsNumberOfBookings()
        {
            var quota = new CallbackBookingQuota() { NumberOfBookings = 3, Quota = 10 };

            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(m => m.GetCallbackBookingQuota(_scheduledAt)).Returns(quota);

            _job.Run(_scheduledAt, null);

            quota.NumberOfBookings.Should().Be(4);

            _mockCrm.Verify(mock => mock.Save(It.Is<CallbackBookingQuota>(q => IsMatch(quota, q))), Times.Once);
            _mockLogger.VerifyInformationWasCalled("ClaimCallbackBookingSlotJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled($"ClaimCallbackBookingSlotJob - Payload {_scheduledAt}");
            _mockLogger.VerifyInformationWasCalled($"ClaimCallbackBookingSlotJob - Succeeded - {_scheduledAt}");
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "ClaimCallbackBookingSlotJob" }).Count.Should().Be(1);
        }


        [Fact]
        public void Run_OnCallbackBookingNotAvailable_DoesNotIncrementNumberOfBookings()
        {
            var quota = new CallbackBookingQuota() { NumberOfBookings = 3, Quota = 3 };

            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(m => m.GetCallbackBookingQuota(_scheduledAt)).Returns(quota);

            _job.Run(_scheduledAt, null);

            _mockCrm.Verify(mock => mock.Save(It.Is<CallbackBookingQuota>(q => IsMatch(quota, q))), Times.Never);
        }

        [Fact]
        public void Run_OnCallbackBookingNotFound_DoesNotIncrementNumberOfBookings()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);
            _mockCrm.Setup(m => m.GetCallbackBookingQuota(_scheduledAt)).Returns<CallbackBookingQuota>(null);

            _job.Run(_scheduledAt, null);

            _mockCrm.Verify(mock => mock.Save(It.IsAny<CallbackBookingQuota>()), Times.Never);
        }

        [Fact]
        public void Run_WhenCrmIntegrationPaused_Aborts()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);

            Action action = () => _job.Run(_scheduledAt, null);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("ClaimCallbackBookingSlotJob - Aborting (CRM integration paused).");
        }

        private static bool IsMatch(object objectA, object objectB)
        {
            objectA.Should().BeEquivalentTo(objectB);
            return true;
        }
    }
}
