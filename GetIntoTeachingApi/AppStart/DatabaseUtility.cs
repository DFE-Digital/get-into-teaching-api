using System.Linq;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace GetIntoTeachingApi.AppStart
{
    public class DatabaseUtility
    {
        private readonly IServiceScope _serviceScope;

        public DatabaseUtility(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
        }

        public void Migrate(IEnv env)
        {
            var dbConfiguration = _serviceScope.ServiceProvider.GetRequiredService<DbConfiguration>();

            if (env.IsMasterInstance)
            {
                dbConfiguration.Migrate();
            }
        }

        public void Seed()
        {
            var dbContext = _serviceScope.ServiceProvider.GetService<GetIntoTeachingDbContext>();

            // Initial CRM sync.
            if (!dbContext.PickListItems.Any())
            {
                RecurringJob.Trigger(JobConfiguration.CrmSyncJobId);
            }

            // Initial locations sync.s
            if (!dbContext.Locations.Any())
            {
                RecurringJob.Trigger(JobConfiguration.LocationSyncJobId);
            }
        }
    }
}
