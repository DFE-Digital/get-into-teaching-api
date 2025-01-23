using System;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.Extensions.Logging;
using ApplyCandidate = GetIntoTeachingApi.Models.Apply.Candidate;

namespace GetIntoTeachingApi.Jobs
{
    public class ApplyCandidateSyncJob : BaseJob
    {
        private readonly ILogger<ApplyCandidateSyncJob> _logger;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ICrmService _crm;
        private readonly Models.IAppSettings _appSettings;

        public ApplyCandidateSyncJob(
            IEnv env,
            IRedisService redis,
            ILogger<ApplyCandidateSyncJob> logger,
            ICrmService crm,
            IBackgroundJobClient jobClient,
            Models.IAppSettings appSettings)
            : base(env, redis)
        {
            _logger = logger;
            _crm = crm;
            _jobClient = jobClient;
            _appSettings = appSettings;
        }

        public void Run(ApplyCandidate applyCandidate)
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException("ApplyCandidateSyncJob - Aborting (CRM integration paused).");
            }

            _logger.LogInformation("ApplyCandidateSyncJob - Started - {Id}", applyCandidate.Id);
            SyncCandidate(applyCandidate);
            _logger.LogInformation("ApplyCandidateSyncJob - Succeeded - {Id}", applyCandidate.Id);
        }

        public void SyncCandidate(ApplyCandidate applyCandidate)
        {
            ContactChannelCandidateWrapper wrappedCandidate = new(applyCandidate.ToCrmModel());
            
            var match = _crm.MatchCandidate(wrappedCandidate.ScopedCandidate.Email, applyCandidate.Id);

            _logger.LogInformation("ApplyCandidateSyncJob - {Status} - {Id}", match == null ? "Miss" : "Hit", applyCandidate.Id);

            if (match != null)
            {
                UpdateCandidateWithMatch(wrappedCandidate.ScopedCandidate, match);
            }
            
            wrappedCandidate.ScopedCandidate.ConfigureChannel(wrappedCandidate, wrappedCandidate.ScopedCandidate.Id);
            
            string json = wrappedCandidate.ScopedCandidate.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));
        }

        private static void UpdateCandidateWithMatch(Models.Crm.Candidate candidate, Models.Crm.Candidate match)
        {
            candidate.Id = match.Id;

            if (candidate.Email == match.Email || match.SecondaryEmail != null)
            {
                return;
            }

            // Write Apply email to the SecondaryEmail field.
            candidate.SecondaryEmail = candidate.Email;
        }
    }
}
