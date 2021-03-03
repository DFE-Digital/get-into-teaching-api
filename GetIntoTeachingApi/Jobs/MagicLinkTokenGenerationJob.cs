using System.Linq;
using System.Text.Json;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace GetIntoTeachingApi.Jobs
{
    public class MagicLinkTokenGenerationJob : BaseJob
    {
        private const int BatchSize = 5000;
        private readonly ICrmService _crm;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ICandidateMagicLinkTokenService _magicLinkTokenService;
        private readonly ILogger<MagicLinkTokenGenerationJob> _logger;
        private readonly IMetricService _metrics;

        public MagicLinkTokenGenerationJob(
            IEnv env,
            IBackgroundJobClient jobClient,
            ICandidateMagicLinkTokenService magicLinkTokenService,
            ICrmService crm,
            ILogger<MagicLinkTokenGenerationJob> logger,
            IMetricService metrics)
            : base(env)
        {
            _jobClient = jobClient;
            _magicLinkTokenService = magicLinkTokenService;
            _crm = crm;
            _logger = logger;
            _metrics = metrics;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 10 * 30)]
        public void Run()
        {
            using (_metrics.MagicLinkTokenGenerationDuration.NewTimer())
            {
                _logger.LogInformation("MagicLinkTokenGenerationJob - Started");
                GenerateTokens();
                _logger.LogInformation("MagicLinkTokenGenerationJob - Succeeded");
            }
        }

        private void GenerateTokens()
        {
            var candidates = _crm.GetCandidatesPendingMagicLinkTokenGeneration(BatchSize);
            _logger.LogInformation($"MagicLinkTokenGenerationJob - Processing ({candidates.Count()})");

            foreach (var candidate in candidates)
            {
                _magicLinkTokenService.GenerateToken(candidate);
                string json = JsonSerializer.Serialize(candidate);
                _jobClient.Enqueue<UpsertCandidateJob>(x => x.Run(json, null));
            }
        }
    }
}
