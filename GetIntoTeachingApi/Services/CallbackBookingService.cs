using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Models.Crm;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Services
{
    public class CallbackBookingService : ICallbackBookingService
    {
        public static readonly int FallbackBookingQuotaWeekdays = 5;
        public static readonly TimeSpan FallbackBookingQuotaInterval = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan FallbackBookingQuotaFirstTimeSpan = new TimeSpan(9, 0, 0);
        public static readonly TimeSpan FallbackBookingQuotaLastTimeSpan = new TimeSpan(16, 30, 0);
        private readonly ICrmService _crm;
        private readonly ILogger<CallbackBookingService> _logger;
        private readonly IDateTimeProvider _dateTime;

        public CallbackBookingService(ICrmService crm, ILogger<CallbackBookingService> logger, IDateTimeProvider dateTime)
        {
            _crm = crm;
            _logger = logger;
            _dateTime = dateTime;
        }

        public IEnumerable<CallbackBookingQuota> GetCallbackBookingQuotas()
        {
            IEnumerable<CallbackBookingQuota> quotas = null;

            try
            {
                 quotas = _crm.GetCallbackBookingQuotas();
            }
            catch
            {
                _logger.LogError("GetCallbackBookingQuotas: failed to reach CRM");
            }

            if (quotas == null || !quotas.Any())
            {
                _logger.LogWarning("GetCallbackBookingQuotas: returning fallback quotas");

                quotas = FallbackBookingQuotas();
            }

            return quotas;
        }

        private static bool IsWeekend(DateTime date)
        {
            var weekendDaysOfWeek = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };

            return weekendDaysOfWeek.Contains(date.DayOfWeek);
        }

        private static IEnumerable<CallbackBookingQuota> GenerateFallbackQuotasOnDay(DateTime day)
        {
            var quotas = new List<CallbackBookingQuota>();
            var startAt = day.Date + FallbackBookingQuotaFirstTimeSpan;

            while (startAt.TimeOfDay <= FallbackBookingQuotaLastTimeSpan)
            {
                var endAt = startAt + FallbackBookingQuotaInterval;

                quotas.Add(CreateFallbackQuota(startAt, endAt));
                startAt = endAt;
            }

            return quotas;
        }

        private static CallbackBookingQuota CreateFallbackQuota(DateTime startAt, DateTime endAt)
        {
            var localStartAt = TimeZoneInfo.ConvertTimeFromUtc(startAt, TimeZoneInfo.Local);
            var localEndAt = TimeZoneInfo.ConvertTimeFromUtc(endAt, TimeZoneInfo.Local);

            return new CallbackBookingQuota()
            {
                Id = Guid.NewGuid(),
                StartAt = startAt,
                EndAt = endAt,
                NumberOfBookings = 0,
                Quota = 1,
                Day = localStartAt.ToLongDateString(),
                TimeSlot = $"{localEndAt.ToShortTimeString()} - {localEndAt.ToShortTimeString()}",
            };
        }

        private IEnumerable<CallbackBookingQuota> FallbackBookingQuotas()
        {
            var quotasByDay = new Dictionary<DateTime, IEnumerable<CallbackBookingQuota>>();
            var day = _dateTime.UtcNow.AddDays(1);

            while (quotasByDay.Count < FallbackBookingQuotaWeekdays)
            {
                if (!IsWeekend(day))
                {
                    quotasByDay.Add(day, GenerateFallbackQuotasOnDay(day));
                }

                day = day.AddDays(1);
            }

            return quotasByDay.Values.SelectMany(q => q);
        }
    }
}