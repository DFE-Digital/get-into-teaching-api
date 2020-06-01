using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Jobs
{
    public class CrmSyncJob : BaseJob
    {
        private readonly ICrmService _crm;
        private readonly IStore _store;

        public CrmSyncJob(ICrmService crm, IStore store)
        {
            _crm = crm;
            _store = store;
        }

        public void Run()
        {
            _store.Sync(_crm);
        }
    }
}
