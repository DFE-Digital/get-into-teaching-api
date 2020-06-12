using System.Threading.Tasks;
using GetIntoTeachingApi.Services;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs
{
    public class CrmSyncJob : BaseJob
    {
        private readonly ICrmService _crm;
        private readonly IStore _store;
        private readonly ILogger<CrmSyncJob> _logger;

        public CrmSyncJob(ICrmService crm, IStore store, ILogger<CrmSyncJob> logger)
        {
            _crm = crm;
            _store = store;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("CrmSyncJob - Started");
            await _store.SyncAsync(_crm);
            _logger.LogInformation("CrmSyncJob - Succeeded");
        }
    }
}
