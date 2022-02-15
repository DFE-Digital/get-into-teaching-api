using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using GetIntoTeachingApi.Models.FindApply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Prometheus;

namespace GetIntoTeachingApi.Jobs
{
    public class FindApplySyncJob : BaseJob
    {
        private readonly ILogger<FindApplySyncJob> _logger;
        private readonly IMetricService _metrics;
        private readonly Models.IAppSettings _appSettings;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IDateTimeProvider _dateTime;

        public FindApplySyncJob(
            IEnv env,
            IRedisService redis,
            ILogger<FindApplySyncJob> logger,
            IBackgroundJobClient jobClient,
            Models.IAppSettings appSettings,
            IMetricService metrics,
            IDateTimeProvider dateTime)
            : base(env, redis)
        {
            _logger = logger;
            _metrics = metrics;
            _appSettings = appSettings;
            _jobClient = jobClient;
            _dateTime = dateTime;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 5)]
        public async Task RunAsync()
        {
            using (_metrics.FindApplySyncDuration.NewTimer())
            {
                _logger.LogInformation("FindApplySyncJob - Started");
                await QueueCandidateSyncJobs();
                _appSettings.FindApplyLastSyncAt = _dateTime.UtcNow;
                _logger.LogInformation("FindApplySyncJob - Succeeded");
            }
        }

        private async Task QueueCandidateSyncJobs()
        {
            var request = Env.FindApplyApiUrl
                .AppendPathSegment("candidates")
                .SetQueryParam("updated_since", UpdatedSince())
                .WithOAuthBearerToken(Env.FindApplyApiKey);

            var paginator = new PaginatorClient<Response<IEnumerable<Candidate>>>(request);

            while (paginator.HasNext)
            {
                var response = await paginator.NextAsync();
                _logger.LogInformation("FindApplySyncJob - Syncing {Count} Candidates", response.Data.Count());
                response.Data.ForEach(c => _jobClient.Enqueue<FindApplyCandidateSyncJob>(x => x.Run(c)));
            }
        }

        private DateTime UpdatedSince()
        {
            // On the initial run we won't get any records back as
            // we ask for those updated since the current time. A separate
            // job/process will back-fill the records updated in the past.
            return _appSettings.FindApplyLastSyncAt ?? _dateTime.UtcNow;
        }
    }
}
