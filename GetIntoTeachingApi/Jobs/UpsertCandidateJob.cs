using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs
{
    public class UpsertCandidateJob : BaseJob
    {
        private readonly ICandidateUpserter _upserter;
        private readonly INotifyService _notifyService;
        private readonly IPerformContextAdapter _contextAdapter;
        private readonly IMetricService _metrics;
        private readonly IAppSettings _appSettings;
        private readonly ILogger<UpsertCandidateJob> _logger;
        private readonly IStore _store;

        public UpsertCandidateJob(
            IEnv env,
            ICandidateUpserter upserter,
            INotifyService notifyService,
            IPerformContextAdapter contextAdapter,
            IMetricService metrics,
            ILogger<UpsertCandidateJob> logger,
            IAppSettings appSettings,
            IStore store)
            : base(env)
        {
            _upserter = upserter;
            _notifyService = notifyService;
            _contextAdapter = contextAdapter;
            _metrics = metrics;
            _logger = logger;
            _appSettings = appSettings;
            _store = store;
        }

        public async Task Run(string json, PerformContext context)
        {
            _logger.LogInformation($"UpsertCandidateJob - Started ({AttemptInfo(context, _contextAdapter)})");
            _logger.LogInformation($"UpsertCandidateJob - Payload {Redactor.RedactJson(json)}");

            var candidate = json.DeserializeChangeTracked<Candidate>();

            if (IsLastAttempt(context, _contextAdapter))
            {
                var personalisation = new Dictionary<string, dynamic>();

                // We fire and forget the email, ensuring the job succeeds.
                await _notifyService.SendEmailAsync(
                    candidate.Email,
                    NotifyService.CandidateRegistrationFailedEmailTemplateId,
                    personalisation);
                await RemoveCandidateFromCache(candidate.Id);
                _logger.LogInformation("UpsertCandidateJob - Deleted");
            }
            else
            {
                _upserter.Upsert(candidate);
                await RemoveCandidateFromCache(candidate.Id);

                _logger.LogInformation($"UpsertCandidateJob - Succeeded - {candidate.Id}");
            }

            var duration = (DateTime.UtcNow - _contextAdapter.GetJobCreatedAt(context)).TotalSeconds;
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertCandidateJob" }).Observe(duration);
        }

        private async Task RemoveCandidateFromCache(Guid? candidateId)
        {
            if (!candidateId.HasValue)
            {
                return;
            }

            var candidate = _store.GetCandidate(candidateId.Value);

            if (candidate != null)
            {
                await _store.DeleteAsync(candidate);
            }
        }
    }
}
