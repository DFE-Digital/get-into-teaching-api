using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Apply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace GetIntoTeachingApi.Jobs
{
    [AutomaticRetry(Attempts = 0)]
    public class ApplyBackfillJob : BaseJob
    {
        public static readonly int PagesPerJob = 10;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger<ApplyBackfillJob> _logger;
        private readonly IAppSettings _appSettings;

        public ApplyBackfillJob(
            IEnv env,
            IRedisService redis,
            IBackgroundJobClient jobClient,
            ILogger<ApplyBackfillJob> logger,
            IAppSettings appSettings)
            : base(env, redis)
        {
            _jobClient = jobClient;
            _logger = logger;
            _appSettings = appSettings;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 60 * 60)]
        public async Task RunAsync(DateTime updatedSince, int startPage = 1)
        {
            _appSettings.IsApplyBackfillInProgress = true;
            _logger.LogInformation("ApplyBackfillJob - Started - Pages {StartPage} to {EndPage}", startPage, EndPage(startPage));
            await QueueCandidateSyncJobs(updatedSince, startPage);
            _logger.LogInformation("ApplyBackfillJob - Succeeded - Pages {StartPage} to {EndPage}", startPage, EndPage(startPage));
            _appSettings.IsApplyBackfillInProgress = false;
        }

        private static int EndPage(int startPage)
        {
            return startPage + PagesPerJob - 1;
        }

        private async Task QueueCandidateSyncJobs(DateTime updatedSince, int startPage)
        {
            var request = Env.ApplyCandidateApiUrl
                .AppendPathSegment("candidates")
                .SetQueryParam("updated_since", updatedSince)
                .WithOAuthBearerToken(Env.ApplyCandidateApiKey);

            var paginator = new PaginatorClient<Response<IEnumerable<Candidate>>>(request, startPage);

            while (paginator.HasNext && paginator.Page <= EndPage(startPage))
            {
                var response = await paginator.NextAsync();
                _logger.LogInformation("ApplyBackfillJob - Syncing {Count} Candidates", response.Data.Count());
                response.Data.ForEach(c => _jobClient.Schedule<ApplyCandidateSyncJob>(x => x.Run(c), TimeSpan.FromHours(2)));
            }

            // When we reach the end page we re-queue the backfill job
            // to process the next batch of pages.
            if (paginator.HasNext)
            {
                _jobClient.Enqueue<ApplyBackfillJob>((x) => x.RunAsync(updatedSince, paginator.Page));
            }
        }
    }
}
