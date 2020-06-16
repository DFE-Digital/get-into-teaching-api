using System.Threading.Tasks;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace GetIntoTeachingApi.Jobs
{
    public class CrmSyncJob : BaseJob
    {
        private readonly ICrmService _crm;
        private readonly IStore _store;
        private readonly ILogger<CrmSyncJob> _logger;
        private readonly IMetricService _metrics;

        public CrmSyncJob(IEnv env, ICrmService crm, IStore store, 
            ILogger<CrmSyncJob> logger, IMetricService metrics)
            : base(env)
        {
            _crm = crm;
            _store = store;
            _logger = logger;
            _metrics = metrics;
        }

        public async Task RunAsync()
        {
            using (_metrics.CrmSyncDuration.NewTimer())
            {
                _logger.LogInformation("CrmSyncJob - Started");
                await _store.SyncAsync(_crm);
                _logger.LogInformation("CrmSyncJob - Succeeded");
            }
        }
    }
}
