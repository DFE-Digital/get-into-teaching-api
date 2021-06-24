using System;
using System.Globalization;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models
{
    public class AppSettings : IAppSettings
    {
        private const string DateFormat = "O";
        private const string CrmOfflineUntilKey = "app_settings.crm_offline_until";
        private const string FindApplyLastSyncAtKey = "app_settings.find_apply_last_sync_at";
        private readonly IRedisService _redis;

        public DateTime? CrmIntegrationPausedUntil
        {
            get
            {
                var dateStr = _redis.Database.StringGet(CrmOfflineUntilKey);

                if (dateStr.IsNullOrEmpty)
                {
                    return null;
                }

                return DateTime.ParseExact(
                    dateStr,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind);
            }

            set
            {
                var dateStr = value?.ToString(DateFormat);

                _redis.Database.StringSet(CrmOfflineUntilKey, dateStr);
            }
        }

        public DateTime? FindApplyLastSyncAt
        {
            get
            {
                var dateStr = _redis.Database.StringGet(FindApplyLastSyncAtKey);

                if (dateStr.IsNullOrEmpty)
                {
                    return null;
                }

                return DateTime.ParseExact(
                    dateStr,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind);
            }

            set
            {
                var dateStr = value?.ToString(DateFormat);

                _redis.Database.StringSet(FindApplyLastSyncAtKey, dateStr);
            }
        }

        public bool IsCrmIntegrationPaused => CrmIntegrationPausedUntil > DateTime.UtcNow;

        public AppSettings(IRedisService redis)
        {
            _redis = redis;
        }
    }
}
