using System;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs
{
    public class ClaimCallbackBookingSlotJob : BaseJob
    {
        private readonly IPerformContextAdapter _contextAdapter;
        private readonly IMetricService _metrics;
        private readonly IAppSettings _appSettings;
        private readonly ICrmService _crm;
        private readonly ILogger<ClaimCallbackBookingSlotJob> _logger;

        public ClaimCallbackBookingSlotJob(
            IEnv env,
            IPerformContextAdapter contextAdapter,
            ICrmService crm,
            IMetricService metrics,
            ILogger<ClaimCallbackBookingSlotJob> logger,
            IAppSettings appSettings)
            : base(env)
        {
            _contextAdapter = contextAdapter;
            _metrics = metrics;
            _logger = logger;
            _appSettings = appSettings;
            _crm = crm;
        }

        public void Run(DateTime scheduledAt, PerformContext context)
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException("ClaimCallbackBookingSlotJob - Aborting (CRM integration paused).");
            }

            _logger.LogInformation("ClaimCallbackBookingSlotJob - Started ({Attempt})", AttemptInfo(context, _contextAdapter));
            _logger.LogInformation("ClaimCallbackBookingSlotJob - Payload {ScheduledAt}", scheduledAt);

            if (IsLastAttempt(context, _contextAdapter))
            {
                // Not critical, they'll still get a call but it may be at a different time
                // so we can just let the job expire.
                _logger.LogInformation($"ClaimCallbackBookingSlotJob - Deleted");
            }
            else
            {
                var quota = _crm.GetCallbackBookingQuota(scheduledAt);

                if (quota != null && quota.IsAvailable)
                {
                    quota.NumberOfBookings += 1;
                    _crm.Save(quota);
                }

                _logger.LogInformation("ClaimCallbackBookingSlotJob - Succeeded - {ScheduledAt}", scheduledAt);
            }

            var duration = (DateTime.UtcNow - _contextAdapter.GetJobCreatedAt(context)).TotalSeconds;
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { $"ClaimCallbackBookingSlotJob" }).Observe(duration);
        }
    }
}
