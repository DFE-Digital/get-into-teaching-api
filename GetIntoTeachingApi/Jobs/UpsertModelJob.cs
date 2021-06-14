using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Jobs
{
    public class UpsertModelJob<T> : BaseJob
        where T : BaseModel, IHasCandidateId
    {
        private readonly IPerformContextAdapter _contextAdapter;
        private readonly IMetricService _metrics;
        private readonly IAppSettings _appSettings;
        private readonly INotifyService _notifyService;
        private readonly ICrmService _crm;
        private readonly ILogger<UpsertModelJob<T>> _logger;

        public UpsertModelJob(
            IEnv env,
            IPerformContextAdapter contextAdapter,
            ICrmService crm,
            IMetricService metrics,
            ILogger<UpsertModelJob<T>> logger,
            IAppSettings appSettings,
            INotifyService notifyService)
            : base(env)
        {
            _contextAdapter = contextAdapter;
            _metrics = metrics;
            _logger = logger;
            _appSettings = appSettings;
            _crm = crm;
            _notifyService = notifyService;
        }

        public void Run(string json, PerformContext context)
        {
            var typeName = typeof(T).Name;

            if (_appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException($"UpsertModelJob<{typeName}> - Aborting (CRM integration paused).");
            }

            _logger.LogInformation($"UpsertModelJob<{typeName}> - Started ({AttemptInfo(context, _contextAdapter)})");
            _logger.LogInformation($"UpsertModelJob<{typeName}> - Payload {Redactor.RedactJson(json)}");

            var model = json.DeserializeChangeTracked<T>();

            if (IsLastAttempt(context, _contextAdapter))
            {
                var candidate = _crm.GetCandidate(model.CandidateId);

                if (candidate != null)
                {
                    var personalisation = new Dictionary<string, dynamic>();

                    // We fire and forget the email, ensuring the job succeeds.
                    _notifyService.SendEmailAsync(
                        candidate.Email,
                        NotifyService.SignUpPartiallyFailedTemplateId,
                        personalisation);
                }

                _logger.LogInformation($"UpsertModelJob<{typeName}> - Deleted");
            }
            else
            {
                _crm.Save(model);

                _logger.LogInformation($"UpsertModelJob<{typeName}> - Succeeded - {model.Id}");
            }

            var duration = (DateTime.UtcNow - _contextAdapter.GetJobCreatedAt(context)).TotalSeconds;
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { $"UpsertModelJob<{typeName}>" }).Observe(duration);
        }
    }
}
