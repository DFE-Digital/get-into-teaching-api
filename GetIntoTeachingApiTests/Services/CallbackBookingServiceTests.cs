using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class CallbackBookingServiceTests
    {
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<ILogger<CallbackBookingService>> _mockLogger;
        private readonly CallbackBookingService _service;

        public CallbackBookingServiceTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _mockLogger = new Mock<ILogger<CallbackBookingService>>();
            _service = new CallbackBookingService(_mockCrm.Object, _mockLogger.Object, new DateTimeProvider());
        }

        [Fact]
        public void GetCallbackBookingQuotas_WhenCrmIsAvailable_ReturnsQuotasFromCrm()
        {
            var mockQuotas = MockQuotas();
            _mockCrm.Setup(m => m.GetCallbackBookingQuotas()).Returns(mockQuotas);

            var quotas = _service.GetCallbackBookingQuotas();

            quotas.Should().BeEquivalentTo(mockQuotas);
        }

        [Fact]
        public void GetCallbackBookingQuotas_WhenCrmIsUnavailable_ReturnsFallbackQuotas()
        {
            _mockCrm.Setup(m => m.GetCallbackBookingQuotas()).Throws(new Exception());

            var quotas = _service.GetCallbackBookingQuotas();

            VerifyFallbackQuotas(quotas);

            _mockLogger.VerifyErrorWasCalled("GetCallbackBookingQuotas: failed to reach CRM");
            _mockLogger.VerifyWarningWasCalled("GetCallbackBookingQuotas: returning fallback quotas");
        }

        [Fact]
        public void GetCallbackBookingQuotas_WhenCrmReturnsEmpty_ReturnsFallbackQuotas()
        {
            _mockCrm.Setup(m => m.GetCallbackBookingQuotas()).Returns(new List<CallbackBookingQuota>());

            var quotas = _service.GetCallbackBookingQuotas();

            VerifyFallbackQuotas(quotas);

            _mockLogger.VerifyWarningWasCalled("GetCallbackBookingQuotas: returning fallback quotas");
        }

        [Fact]
        public void GetCallbackBookingQuotas_WhenCrmReturnsNull_ReturnsFallbackQuotas()
        {
            _mockCrm.Setup(m => m.GetCallbackBookingQuotas()).Returns<IEnumerable<CallbackBookingQuota>>(null);

            var quotas = _service.GetCallbackBookingQuotas();

            VerifyFallbackQuotas(quotas);

            _mockLogger.VerifyWarningWasCalled("GetCallbackBookingQuotas: returning fallback quotas");
        }

        private static IEnumerable<CallbackBookingQuota> MockQuotas()
        {
            var quota1 = new CallbackBookingQuota() { Id = Guid.NewGuid() };

            return new [] { quota1 };
        }

        private static void VerifyFallbackQuotas(IEnumerable<CallbackBookingQuota> quotas)
        {
            var timeSpanPerDay = CallbackBookingService.FallbackBookingQuotaLastTimeSpan - CallbackBookingService.FallbackBookingQuotaFirstTimeSpan;
            var expectedQuotasPerDay = ((timeSpanPerDay / CallbackBookingService.FallbackBookingQuotaInterval) + 1);
            var expectedQuotas = CallbackBookingService.FallbackBookingQuotaWeekdays * expectedQuotasPerDay;
            quotas.Count().Should().Be((int)expectedQuotas);

            quotas.Select(q => q.Id).Should().NotContain(id => id == null);
            quotas.Select(q => q.IsAvailable).Should().OnlyContain(b => b == true);
            quotas.Select(q => q).Should().OnlyContain(q => q.EndAt == q.StartAt + TimeSpan.FromMinutes(30));
            quotas.Select(q => q).Should().OnlyContain(q => VerifyDay(q));
            quotas.Select(q => q).Should().OnlyContain(q => VerifyTimeSlot(q));
            quotas.First().StartAt.TimeOfDay.Should().Be(new TimeSpan(9, 0, 0));
            quotas.Last().StartAt.TimeOfDay.Should().Be(new TimeSpan(16, 30, 0));

            var dates = quotas.Select(q => q.StartAt.Date).Distinct();
            dates.Count().Should().Be(CallbackBookingService.FallbackBookingQuotaWeekdays);
            dates.Should().NotContain(d => d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday);
        }

        private static bool VerifyTimeSlot(CallbackBookingQuota quota)
        {
            var localEndAt = TimeZoneInfo.ConvertTimeFromUtc(quota.EndAt, TimeZoneInfo.Local);

            return quota.TimeSlot == $"{localEndAt.ToShortTimeString()} - {localEndAt.ToShortTimeString()}";
        }

        private static bool VerifyDay(CallbackBookingQuota quota)
        {
            var localStartAt = TimeZoneInfo.ConvertTimeFromUtc(quota.StartAt, TimeZoneInfo.Local);

            return quota.Day == localStartAt.ToLongDateString();
        }
    }
}
