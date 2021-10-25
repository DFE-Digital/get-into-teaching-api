using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.FindApply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace GetIntoTeachingApi.Jobs
{
    public class FindApplyBackfillJob : BaseJob
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger<FindApplyBackfillJob> _logger;
        private readonly IAppSettings _appSettings;

        public FindApplyBackfillJob(
            IEnv env,
            IBackgroundJobClient jobClient,
            ILogger<FindApplyBackfillJob> logger,
            IAppSettings appSettings)
            : base(env)
        {
            _jobClient = jobClient;
            _logger = logger;
            _appSettings = appSettings;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 60 * 60)]
        public async Task RunAsync()
        {
            _appSettings.IsFindApplyBackfillInProgress = true;
            _logger.LogInformation("FindApplyBackfillJob - Started");
            await QueueCandidateSyncJobs();
            _logger.LogInformation("FindApplyBackfillJob - Succeeded");
            _appSettings.IsFindApplyBackfillInProgress = false;
        }

        private async Task QueueCandidateSyncJobs()
        {
            var request = Env.FindApplyApiUrl
                .AppendPathSegment("candidates")
                .SetQueryParam("updated_since", DateTime.MinValue)
                .WithOAuthBearerToken(Env.FindApplyApiKey);

            var paginator = new PaginatorClient<Response<IEnumerable<Candidate>>>(request);

            while (paginator.HasNext)
            {
                var response = await paginator.NextAsync();
                _logger.LogInformation($"FindApplyBackfillJob - Syncing {response.Data.Count()} Candidates");
                response.Data.ForEach(c => _jobClient.Schedule<FindApplyCandidateSyncJob>(x => x.Run(c), TimeSpan.FromMinutes(30)));
            }
        }
    }
}
