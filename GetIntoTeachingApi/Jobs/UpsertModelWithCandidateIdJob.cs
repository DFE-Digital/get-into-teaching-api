using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Jobs.CandidateSanitisation;
using GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;

namespace GetIntoTeachingApi.Jobs
{
    public class UpsertModelWithCandidateIdJob<T> : BaseJob
        where T : BaseModel, IHasCandidateId
    {
        private readonly IPerformContextAdapter _contextAdapter;
        private readonly IMetricService _metrics;
        private readonly IAppSettings _appSettings;
        private readonly INotifyService _notifyService;
        private readonly ICrmService _crm;
        private readonly ILogger<UpsertModelWithCandidateIdJob<T>> _logger;
        private readonly ICrmModelSanitisationRulesHandler<ContactChannelCreationSanitisationRequestWrapper> _rulesHandler;

        public UpsertModelWithCandidateIdJob(
            IEnv env,
            IRedisService redis,
            IPerformContextAdapter contextAdapter,
            ICrmService crm,
            IMetricService metrics,
            ILogger<UpsertModelWithCandidateIdJob<T>> logger,
            IAppSettings appSettings,
            INotifyService notifyService,
            ICrmModelSanitisationRulesHandler<ContactChannelCreationSanitisationRequestWrapper> rulesHandler)
            : base(env, redis)
        {
            _contextAdapter = contextAdapter;
            _metrics = metrics;
            _logger = logger;
            _appSettings = appSettings;
            _crm = crm;
            _notifyService = notifyService;
            _rulesHandler = rulesHandler;
        }

        public void Run(string json, PerformContext context)
        {
            var typeName = typeof(T).Name;

            if (_appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException($"UpsertModelJob<{typeName}> - Aborting (CRM integration paused).");
            }

            _logger.LogInformation("UpsertModelJob<{TypeName}> - Started ({Attempt})", typeName, AttemptInfo(context, _contextAdapter));
            _logger.LogInformation("UpsertModelJob<{TypeName}> - Payload {Payload}", typeName, Redactor.RedactJson(json));

            var model = json.DeserializeChangeTracked<T>();
            
            //we need to ensure we retrieve the creation channels, we should not accept a timeout error
            Candidate candidate = _crm.GetCandidate(model.CandidateId);

            if (IsLastAttempt(context, _contextAdapter))
            {
                if (candidate != null)
                {
                    var personalisation = new Dictionary<string, dynamic>();

                    // We fire and forget the email, ensuring the job succeeds.
                    _notifyService.SendEmailAsync(
                        candidate.Email,
                        NotifyService.SignUpPartiallyFailedTemplateId,
                        personalisation);
                }

                _logger.LogInformation("UpsertModelJob<{TypeName}> - Deleted", typeName);
            }
            else
            {
                // TODO: Sanitise this model before saving
                // Candidate candidate = _crm.GetCandidate(model.CandidateId);
                // candidate.ContactChannelCreations
                // Call Handler 
                // if wrapper.preserve == true then save

                if (typeof(T) == typeof(ContactChannelCreation))
                {
                    ContactChannelCreation creationChannel = model as ContactChannelCreation;
                    ContactChannelCreationSanitisationRequestWrapper wrapper = ContactChannelCreationSanitisationRequestWrapper.Create(creationChannel, candidate.ContactChannelCreations.AsReadOnly());
                    wrapper = _rulesHandler.SanitiseCrmModelWithRules(wrapper);

                    if (wrapper.Preserve)
                    {
                        _crm.Save(model);
                        _logger.LogInformation("UpsertModelJob<{TypeName}> - Succeeded - {Id}", typeName, model.Id);
                    }
                }
                else
                {
                    _crm.Save(model);
                    _logger.LogInformation("UpsertModelJob<{TypeName}> - Succeeded - {Id}", typeName, model.Id);
                }
            }

            var duration = (DateTime.UtcNow - _contextAdapter.GetJobCreatedAt(context)).TotalSeconds;
            _metrics.HangfireJobQueueDuration.WithLabels($"UpsertModelJob<{typeName}>").Observe(duration);
        }
    }
}
