using System;
using GetIntoTeachingApi.Models.FindApply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs
{
    public class FindApplyCandidateSyncJob : BaseJob
    {
        private readonly ILogger<FindApplyCandidateSyncJob> _logger;
        private readonly ICrmService _crm;
        private readonly Models.IAppSettings _appSettings;

        public FindApplyCandidateSyncJob(
            IEnv env,
            ILogger<FindApplyCandidateSyncJob> logger,
            ICrmService crm,
            Models.IAppSettings appSettings)
            : base(env)
        {
            _logger = logger;
            _crm = crm;
            _appSettings = appSettings;
        }

        public void Run(Candidate candidate)
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException("FindApplyCandidateSyncJob - Aborting (CRM integration paused).");
            }

            _logger.LogInformation($"FindApplyCandidateSyncJob - Started - {candidate.Id}");
            SyncCandidate(candidate);
            _logger.LogInformation($"FindApplyCandidateSyncJob - Succeeded - {candidate.Id}");
        }

        public void SyncCandidate(Candidate candidate)
        {
            var match = _crm.MatchCandidate(candidate.Attributes.Email);

            if (match != null)
            {
                _logger.LogInformation($"FindApplyCandidateSyncJob - Hit - {candidate.Id}");
                match.FindApplyId = candidate.Id;
                _crm.Save(match);
            }
            else
            {
                _logger.LogInformation($"FindApplyCandidateSyncJob - Miss - {candidate.Id}");
            }
        }
    }
}
