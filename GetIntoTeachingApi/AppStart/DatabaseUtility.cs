using System.Linq;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace GetIntoTeachingApi.AppStart
{
    public static class DatabaseUtility
    {
        public static void Migrate(IServiceScope scope, IEnv env)
        {
            var dbConfiguration = scope.ServiceProvider.GetRequiredService<DbConfiguration>();

            if (env.IsMasterInstance)
            {
                dbConfiguration.Migrate();
            }
        }

        public static void Seed(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetService<GetIntoTeachingDbContext>();

            // Initial CRM sync.
            if (!dbContext.PickListItems.Any())
            {
                RecurringJob.Trigger(JobConfiguration.CrmSyncJobId);
            }

            // Initial location sync.
            if (!dbContext.Locations.Any())
            {
                RecurringJob.Trigger(JobConfiguration.LocationSyncJobId);
            }
        }
    }
}