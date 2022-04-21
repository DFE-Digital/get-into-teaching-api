using System;
using GetIntoTeachingApi.Models.FindApply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs
{
    public class FindApplyCandidateSyncV12Job : BaseJob
    {
        private readonly ILogger<FindApplyCandidateSyncJob> _logger;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ICrmService _crm;
        private readonly Models.IAppSettings _appSettings;

        public FindApplyCandidateSyncV12Job(
            IEnv env,
            IRedisService redis,
            ILogger<FindApplyCandidateSyncJob> logger,
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

        public void Run(Candidate findApplyCandidate)
        {
            if (_appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException("FindApplyCandidateSyncV12Job - Aborting (CRM integration paused).");
            }

            _logger.LogInformation("FindApplyCandidateSyncV12Job - Started - {Id}", findApplyCandidate.Id);
            SyncCandidate(findApplyCandidate);
            _logger.LogInformation("FindApplyCandidateSyncV12Job - Succeeded - {Id}", findApplyCandidate.Id);
        }

        public void SyncCandidate(Candidate findApplyCandidate)
        {
            var candidate = findApplyCandidate.ToCrmModel();
            var match = _crm.MatchCandidate(candidate.Email);

            _logger.LogInformation("FindApplyCandidateSyncV12Job - {Status} - {Id}", match == null ? "Miss" : "Hit", findApplyCandidate.Id);

            if (match != null)
            {
                candidate.Id = match.Id;
            }
            else
            {
                candidate.ChannelId = (int)Models.Crm.Candidate.Channel.ApplyForTeacherTraining;
            }

            string json = candidate.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));
        }
    }
}
