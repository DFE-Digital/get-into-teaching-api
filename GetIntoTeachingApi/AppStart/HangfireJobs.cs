﻿using GetIntoTeachingApi.Jobs;
using Hangfire;

namespace GetIntoTeachingApi.AppStart
{
    public static class HangfireJobs
    {
        public static void AddCrmSyncJob()
        {
            const string everyFifthMinute = "*/5 * * * *";
            RecurringJob.AddOrUpdate<CrmSyncJob>(JobConfiguration.CrmSyncJobId, (x) => x.RunAsync(), everyFifthMinute);
        }

        public static void AddLocationSyncJob()
        {
            RecurringJob.AddOrUpdate<LocationSyncJob>(
                    JobConfiguration.LocationSyncJobId,
                    (x) => x.RunAsync(LocationSyncJob.FreeMapToolsUrl),
                    Cron.Weekly());
        }

        public static void AddMagicLinkTokenGenerationJob()
        {
            RecurringJob.AddOrUpdate<MagicLinkTokenGenerationJob>(
                    JobConfiguration.MagicLinkTokenGenerationJobId,
                    (x) => x.Run(),
                    Cron.Hourly());
        }

        public static void AddApplySyncJob()
        {
            RecurringJob.AddOrUpdate<ApplySyncJob>(
                                    JobConfiguration.ApplySyncJobId,
                                    (x) => x.RunAsync(),
                                    Cron.Hourly());
        }

        public static void RemoveApplySyncJob()
        {
            RecurringJob.RemoveIfExists(JobConfiguration.ApplySyncJobId);
        }
    }
}