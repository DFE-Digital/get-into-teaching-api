using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class AppSettingsTests
    {
        private readonly AppSettings _settings;
        private readonly Mock<IDatabase> _database;

        public AppSettingsTests()
        {
            _database = new Mock<IDatabase>();
            var redis = new Mock<IRedisService>();
            redis.Setup(m => m.Database).Returns(_database.Object);

            _settings = new AppSettings(redis.Object);
        }

        [Fact]
        public void CrmIntegrationPausedUntil_SetAndGetWithDate_WorkCorrectly()
        {
            var date = DateTime.UtcNow;
            var dateStr = date.ToString("O");
            _database.Setup(m => m.KeyExists("app_settings.crm_offline_until", CommandFlags.None)).Returns(true);
            _database.Setup(m => m.StringGet("app_settings.crm_offline_until", CommandFlags.None)).Returns(dateStr);

            _settings.CrmIntegrationPausedUntil = date;
            _settings.CrmIntegrationPausedUntil.Should().Be(date);

            _database.Verify(m => m.StringSet("app_settings.crm_offline_until", dateStr, null, When.Always, CommandFlags.None));
        }

        [Fact]
        public void CrmIntegrationPausedUntil_SetAndGetWithNull_WorkCorrectly()
        {
            _database.Setup(m => m.KeyExists("app_settings.crm_offline_until", CommandFlags.None)).Returns(true);
            _database.Setup(m => m.StringGet("app_settings.crm_offline_until", CommandFlags.None)).Returns(null as string);

            _settings.CrmIntegrationPausedUntil = null;
            _settings.CrmIntegrationPausedUntil.Should().BeNull();

            _database.Verify(m => m.StringSet("app_settings.crm_offline_until", null as string, null, When.Always, CommandFlags.None));
        }

        [Fact]
        public void IsCrmIntegrationPaused_WhenNotYetSet_ReturnsFalse()
        {
            _database.Setup(m => m.KeyExists("app_settings.crm_offline_until", CommandFlags.None)).Returns(false);

            _settings.IsCrmIntegrationPaused.Should().BeFalse();
        }

        [Fact]
        public void IsCrmIntegrationPaused_WhenCrmOfflineUntilIsAFutureDate_ReturnsTrue()
        {
            _database.Setup(m => m.KeyExists("app_settings.crm_offline_until", CommandFlags.None)).Returns(true);
            var dateStr = DateTime.UtcNow.AddDays(1).ToString("O");
            _database.Setup(m => m.StringGet("app_settings.crm_offline_until", CommandFlags.None)).Returns(dateStr);

            _settings.IsCrmIntegrationPaused.Should().BeTrue();
        }

        [Fact]
        public void IsCrmIntegrationPaused_WhenCrmOfflineUntilIsAPastDate_ReturnsFalse()
        {
            _database.Setup(m => m.KeyExists("app_settings.crm_offline_until", CommandFlags.None)).Returns(true);
            var dateStr = DateTime.UtcNow.AddDays(-1).ToString("O");
            _database.Setup(m => m.StringGet("app_settings.crm_offline_until", CommandFlags.None)).Returns(dateStr);

            _settings.IsCrmIntegrationPaused.Should().BeFalse();
        }

        [Fact]
        public void FindApplyLastSyncAt_SetAndGetWithDate_WorkCorrectly()
        {
            var date = DateTime.UtcNow;
            var dateStr = date.ToString("O");
            _database.Setup(m => m.KeyExists("app_settings.find_apply_last_sync_at", CommandFlags.None)).Returns(true);
            _database.Setup(m => m.StringGet("app_settings.find_apply_last_sync_at", CommandFlags.None)).Returns(dateStr);

            _settings.FindApplyLastSyncAt = date;
            _settings.FindApplyLastSyncAt.Should().Be(date);

            _database.Verify(m => m.StringSet("app_settings.find_apply_last_sync_at", dateStr, null, When.Always, CommandFlags.None));
        }

        [Fact]
        public void FindApplyLastSyncAt_SetAndGetWithNull_WorkCorrectly()
        {
            _database.Setup(m => m.KeyExists("app_settings.find_apply_last_sync_at", CommandFlags.None)).Returns(true);
            _database.Setup(m => m.StringGet("app_settings.find_apply_last_sync_at", CommandFlags.None)).Returns(null as string);

            _settings.FindApplyLastSyncAt = null;
            _settings.FindApplyLastSyncAt.Should().BeNull();

            _database.Verify(m => m.StringSet("app_settings.find_apply_last_sync_at", null as string, null, When.Always, CommandFlags.None));
        }

        [Fact]
        public void FindApplyLastSyncAt_WhenNotYetSet_ReturnsFalse()
        {
            _database.Setup(m => m.KeyExists("app_settings.find_apply_last_sync_at", CommandFlags.None)).Returns(false);

            _settings.FindApplyLastSyncAt.Should().BeNull();
        }

        [Fact]
        public void FindApplyBackfillInProgress_SetAndGet_WorkCorrectly()
        {
            _database.Setup(m => m.KeyExists("app_settings.find_apply_backfill_in_progress", CommandFlags.None)).Returns(true);
            _database.Setup(m => m.StringGet("app_settings.find_apply_backfill_in_progress", CommandFlags.None)).Returns(null as string);
            _settings.IsFindApplyBackfillInProgress.Should().BeFalse();

            _database.Setup(m => m.StringGet("app_settings.find_apply_backfill_in_progress", CommandFlags.None)).Returns("True");
            _settings.IsFindApplyBackfillInProgress.Should().BeTrue();

            _database.Setup(m => m.StringGet("app_settings.find_apply_backfill_in_progress", CommandFlags.None)).Returns("False");
            _settings.IsFindApplyBackfillInProgress.Should().BeFalse();

            _settings.IsFindApplyBackfillInProgress = false;
            _database.Verify(m => m.StringSet("app_settings.find_apply_backfill_in_progress", "False", null, When.Always, CommandFlags.None));

            _settings.IsFindApplyBackfillInProgress = true;
            _database.Verify(m => m.StringSet("app_settings.find_apply_backfill_in_progress", "True", null, When.Always, CommandFlags.None));

            _database.Setup(m => m.KeyExists("app_settings.find_apply_backfill_in_progress", CommandFlags.None)).Returns(false);
            _settings.IsFindApplyBackfillInProgress.Should().BeFalse();
        }
    }
}

