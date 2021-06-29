using System.Linq;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Jobs;
using Hangfire;

namespace GetIntoTeachingApi.AppStart
{
    public static class DatabaseUtility
    {
        //public static void Seed(service)
        //{
        //    var dbContext = serviceScope.ServiceProvider.GetService<GetIntoTeachingDbContext>();

        //    // Initial CRM sync.
        //    if (!dbContext.PickListItems.Any())
        //    {
        //        RecurringJob.Trigger(JobConfiguration.CrmSyncJobId);
        //    }

        //    // Initial locations sync.s
        //    if (!dbContext.Locations.Any())
        //    {
        //        RecurringJob.Trigger(JobConfiguration.LocationSyncJobId);
        //    }
        //}
    }
}
