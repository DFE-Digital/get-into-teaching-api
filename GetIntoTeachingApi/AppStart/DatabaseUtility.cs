using System.Linq;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Services;
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
                RecurringJob.TriggerJob(JobConfiguration.CrmSyncJobId);
            }

            // Initial location sync.
            if (!dbContext.Locations.Any())
            {
                RecurringJob.TriggerJob(JobConfiguration.LocationSyncJobId);
            }
        }

        // Temporary method to ensure the new lookup item table for Country
        // and TeachingSubject are populated prior to the application booting.
        public static void SeedCountriesAndTeachingSubjects(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetService<GetIntoTeachingDbContext>();

            if (dbContext.Countries.Any() && dbContext.TeachingSubjects.Any())
            {
                return;
            }

            var store = scope.ServiceProvider.GetService<IStore>();

            store.SyncAsync().Wait();
        }
    }
}