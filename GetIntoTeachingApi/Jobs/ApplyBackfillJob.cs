using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Apply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace GetIntoTeachingApi.Jobs
{
    public class ApplyBackfillJob : BaseJob
    {
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
        public async Task RunAsync()
        {
            _appSettings.IsApplyBackfillInProgress = true;
            _logger.LogInformation("ApplyBackfillJob - Started");
            await QueueCandidateSyncJobs();
            _logger.LogInformation("ApplyBackfillJob - Succeeded");
            _appSettings.IsApplyBackfillInProgress = false;
        }

        private async Task QueueCandidateSyncJobs()
        {
            var request = Env.ApplyCandidateApiUrl
                .AppendPathSegment("candidates")
                .SetQueryParam("updated_since", DateTime.MinValue)
                .WithOAuthBearerToken(Env.ApplyCandidateApiKey);

            var paginator = new PaginatorClient<Response<IEnumerable<Candidate>>>(request);

            while (paginator.HasNext)
            {
                var response = await paginator.NextAsync();
                _logger.LogInformation("ApplyBackfillJob - Syncing {Count} Candidates", response.Data.Count());
                response.Data.ForEach(c => _jobClient.Schedule<ApplyCandidateSyncJob>(x => x.Run(c), TimeSpan.FromMinutes(30)));
            }
        }
    }
}
