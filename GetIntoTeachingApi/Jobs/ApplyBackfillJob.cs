using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Newtonsoft;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Apply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Hangfire.Server;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using MoreLinq;
using NuGet.Protocol;

namespace GetIntoTeachingApi.Jobs
{
    [AutomaticRetry(Attempts = 0)]
    public class ApplyBackfillJob : BaseJob
    {
        public static readonly int PagesPerJob = 10;
        public static readonly int RecordsPerJob = 20;
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
        public async Task RunAsync(DateTime updatedSince, int startPage = 1, IEnumerable<int> candidateIds = null)
        {
            _appSettings.IsApplyBackfillInProgress = true;
            _logger.LogInformation("ApplyBackfillJob - Started - Pages {StartPage} to {EndPage} (candidate IDs: {CandidateIds})", startPage, EndPage(startPage), candidateIds?.Any());

            await (candidateIds?.Any() ?? false
                ? QueueCandidateSyncJobsCandidateIds(candidateIds)
                : QueueCandidateSyncJobsUpdatedSince(updatedSince, startPage));

            _logger.LogInformation("ApplyBackfillJob - Succeeded - Pages {StartPage} to {EndPage} (candidate IDs: {CandidateIds})", startPage, EndPage(startPage), candidateIds?.Any());
            _appSettings.IsApplyBackfillInProgress = false;
        }

        private static int EndPage(int startPage)
        {
            return startPage + PagesPerJob - 1;
        }

        private async Task QueueCandidateSyncJobsUpdatedSince(DateTime updatedSince, int startPage)
        {
            // Enforce use of the Newtonsoft Json serializer
            FlurlHttp.Clients.UseNewtonsoft();

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
                _jobClient.Enqueue<ApplyBackfillJob>((x) => x.RunAsync(updatedSince, paginator.Page, null));
            }
        }
        
        private async Task QueueCandidateSyncJobsCandidateIds(IEnumerable<int> candidateIds)
        {
            // Enforce use of the Newtonsoft Json serializer
            FlurlHttp.Clients.UseNewtonsoft();
            
            var batch = candidateIds.Take(RecordsPerJob);
            var remainder = candidateIds.Skip(RecordsPerJob);

            foreach (int candidateId in batch) 
            {
                _logger.LogInformation("Fetching CandidateID C{CandidateId} from the Apply API", candidateId);

                try
                {
                    var request = Env.ApplyCandidateApiUrl
                        .AppendPathSegment("candidates")
                        .AppendPathSegment(String.Format(CultureInfo.InvariantCulture, "C{0}", candidateId))
                        .WithOAuthBearerToken(Env.ApplyCandidateApiKey);

                    var candidate = await request.GetJsonAsync<Response<GetIntoTeachingApi.Models.Apply.Candidate>>();

                    _logger.LogInformation("Scheduling ApplyBackfillJob - Syncing CandidateID: C{Id}", candidateId);
                    
                    _jobClient.Schedule<ApplyCandidateSyncJob>(x => x.Run(candidate.Data), TimeSpan.FromSeconds(60));
                }
                catch (FlurlHttpException ex)
                {
                    _logger.LogError("Failed to fetch CandidateID C{CandidateId} from the Apply API (status: {Status})", candidateId, ex.StatusCode);
                }

                await Task.Delay(100); // add a short delay so as not to overwhelm the Apply API
            }
            
            // When we reach the end page we re-queue the backfill job
            // to process the next batch of candidate IDs.
            if (remainder?.Any() ?? false)
            {
                _jobClient.Enqueue<ApplyBackfillJob>((x) => x.RunAsync(DateTime.MinValue, 1, remainder.ToArray()));
            }
        }
    }
}
