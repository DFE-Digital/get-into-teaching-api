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

        public void Run(Candidate findApplyCandidate)
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException("FindApplyCandidateSyncJob - Aborting (CRM integration paused).");
            }

            _logger.LogInformation($"FindApplyCandidateSyncJob - Started - {findApplyCandidate.Id}");
            SyncCandidate(findApplyCandidate);
            _logger.LogInformation($"FindApplyCandidateSyncJob - Succeeded - {findApplyCandidate.Id}");
        }

        public void SyncCandidate(Candidate findApplyCandidate)
        {
            var match = _crm.MatchCandidate(findApplyCandidate.Attributes.Email);

            if (match != null)
            {
                _logger.LogInformation($"FindApplyCandidateSyncJob - Hit - {findApplyCandidate.Id}");

                // We persist a new Candidate to ensure we only write the FindApplyId
                // back to the CRM and not existing attributes on the match.
                var candidate = new Models.Crm.Candidate() { Id = match.Id, FindApplyId = findApplyCandidate.Id };
                _crm.Save(candidate);
            }
            else
            {
                _logger.LogInformation($"FindApplyCandidateSyncJob - Miss - {findApplyCandidate.Id}");
            }
        }
    }
}
