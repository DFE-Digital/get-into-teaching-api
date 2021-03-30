using System.Threading.Tasks;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace GetIntoTeachingApi.Jobs
{
    public class CrmSyncJob : BaseJob
    {
        private readonly IStore _store;
        private readonly ILogger<CrmSyncJob> _logger;
        private readonly IMetricService _metrics;
        private readonly IAppSettings _appSettings;

        public CrmSyncJob(
            IEnv env,
            IStore store,
            ILogger<CrmSyncJob> logger,
            IMetricService metrics,
            IAppSettings appSettings)
            : base(env)
        {
            _store = store;
            _logger = logger;
            _metrics = metrics;
            _appSettings = appSettings;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
        public async Task RunAsync()
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                _logger.LogInformation("CrmSyncJob - Skipping (CRM integration paused)");
                return;
            }

            using (_metrics.CrmSyncDuration.NewTimer())
            {
                _logger.LogInformation("CrmSyncJob - Started");
                await _store.SyncAsync();
                _logger.LogInformation("CrmSyncJob - Succeeded");
            }
        }
    }
}
