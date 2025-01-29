using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs.FilterAttributes;
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

        public UpsertCandidateJob(
            IEnv env,
            IRedisService redis,
            ICandidateUpserter upserter,
            INotifyService notifyService,
            IPerformContextAdapter contextAdapter,
            IMetricService metrics,
            ILogger<UpsertCandidateJob> logger,
            IAppSettings appSettings)
            : base(env, redis)
        {
            _upserter = upserter;
            _notifyService = notifyService;
            _contextAdapter = contextAdapter;
            _metrics = metrics;
            _logger = logger;
            _appSettings = appSettings;
        }

        [CorrelationIdFilter]
        public void Run(string json, PerformContext context)
        {
            var candidate = json.DeserializeChangeTracked<Candidate>();
            Guid correlationId = GetCorrelationId(context);

            if (Deduplicate(Signature(candidate), context, _contextAdapter))
            {
                _logger.LogInformation("UpsertCandidateJob - Deduplicating ({CorrelationId})", correlationId);
                return;
            }

            if (_appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException(
                    $"UpsertCandidateJob - Aborting (CRM integration paused).");
            }

            _logger.LogInformation("UpsertCandidateJob - Started ({Attempt}) {CorrelationId}", AttemptInfo(context, _contextAdapter), correlationId);
            _logger.LogInformation("UpsertCandidateJob - Payload {Payload} {CorrelationId}", Redactor.RedactJson(json), correlationId);

            if (IsLastAttempt(context, _contextAdapter))
            {
                var personalisation = new Dictionary<string, dynamic>();

                // We fire and forget the email, ensuring the job succeeds.
                _notifyService.SendEmailAsync(
                    candidate.Email,
                    NotifyService.CandidateRegistrationFailedEmailTemplateId,
                    personalisation);
                _logger.LogInformation("UpsertCandidateJob - Deleted ({CorrelationId})", correlationId);
            }
            else
            {
                _upserter.Upsert(candidate);

                _logger.LogInformation("UpsertCandidateJob - Succeeded - {Id} {CorrelationId}", candidate.Id, correlationId);
            }

            var duration = (DateTime.UtcNow - _contextAdapter.GetJobCreatedAt(context)).TotalSeconds;
            _metrics.HangfireJobQueueDuration.WithLabels("UpsertCandidateJob").Observe(duration);
        }

        private static string Signature(Candidate candidate)
        {
            return $"{candidate.Id}-{candidate.Email}-{string.Join("", candidate.ChangedPropertyNames)}";
        }
    }
}
