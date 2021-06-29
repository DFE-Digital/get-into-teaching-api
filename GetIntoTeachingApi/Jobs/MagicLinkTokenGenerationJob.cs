using System.Linq;
using GetIntoTeachingApi.Models;
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
        private readonly IAppSettings _appSettings;

        public MagicLinkTokenGenerationJob(
            IEnv env,
            IBackgroundJobClient jobClient,
            ICandidateMagicLinkTokenService magicLinkTokenService,
            ICrmService crm,
            ILogger<MagicLinkTokenGenerationJob> logger,
            IMetricService metrics,
            IAppSettings appSettings)
            : base(env)
        {
            _jobClient = jobClient;
            _magicLinkTokenService = magicLinkTokenService;
            _crm = crm;
            _logger = logger;
            _appSettings = appSettings;
            _metrics = metrics;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 60)]
        public void Run()
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                _logger.LogInformation("MagicLinkTokenGenerationJob - Skipping (CRM integration paused)");
                return;
            }

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
                string json = candidate.SerializeChangeTracked();
                _jobClient.Enqueue<UpsertCandidateJob>(x => x.Run(json, null));
            }
        }
    }
}
