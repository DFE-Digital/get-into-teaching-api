using System;
using System.Globalization;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;

namespace GetIntoTeachingApi.Models
{
    public class AppSettings : IAppSettings
    {
        private const string DateFormat = "O";
        private const string CrmOfflineUntilKey = "app_settings.crm_offline_until";
        private const string ApplyLastSyncAtKey = "app_settings.apply_last_sync_at";
        private const string ApplyBackfillInProgressKey = "app_settings.apply_backfill_in_progress";
        private readonly IRedisService _redis;

        public DateTime? CrmIntegrationPausedUntil
        {
            get
            {
                if (!_redis.Database.KeyExists(CrmOfflineUntilKey))
                {
                    return null;
                }

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

        public DateTime? ApplyLastSyncAt
        {
            get
            {
                if (!_redis.Database.KeyExists(ApplyLastSyncAtKey))
                {
                    return null;
                }

                var dateStr = _redis.Database.StringGet(ApplyLastSyncAtKey);

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

                _redis.Database.StringSet(ApplyLastSyncAtKey, dateStr);
            }
        }

        public bool IsApplyBackfillInProgress
        {
            get
            {
                if (!_redis.Database.KeyExists(ApplyBackfillInProgressKey))
                {
                    return false;
                }

                var backfillString = _redis.Database.StringGet(ApplyBackfillInProgressKey).ToString();

                return backfillString.ToBool();
            }

            set
            {
                _redis.Database.StringSet(ApplyBackfillInProgressKey, value.ToString());
            }
        }

        public bool IsCrmIntegrationPaused => CrmIntegrationPausedUntil > DateTime.UtcNow;

        public AppSettings(IRedisService redis)
        {
            _redis = redis;
        }
    }
}
