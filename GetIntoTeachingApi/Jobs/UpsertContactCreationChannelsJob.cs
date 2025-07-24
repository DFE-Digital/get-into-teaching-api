using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs.UpsertStrategies;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Jobs;


public class UpsertContactCreationChannelsJob : BaseJob
{
    private readonly IPerformContextAdapter _contextAdapter;
    private readonly IMetricService _metrics;
    private readonly IAppSettings _appSettings;
    private readonly INotifyService _notifyService;
    private readonly ICrmService _crm;
    private readonly ILogger<UpsertContactCreationChannelsJob> _logger;
    private readonly ICrmlUpsertStrategy<ContactChannelCreation> _crmUpsertStrategy;


    public UpsertContactCreationChannelsJob(
        IEnv env,
        IRedisService redis,
        IPerformContextAdapter contextAdapter,
        ICrmService crm,
        IMetricService metrics,
        ILogger<UpsertContactCreationChannelsJob> logger,
        IAppSettings appSettings,
        INotifyService notifyService,
        ICrmlUpsertStrategy<ContactChannelCreation> crmUpsertStrategy)
        : base(env, redis)
    {
        _contextAdapter = contextAdapter;
        _metrics = metrics;
        _logger = logger;
        _appSettings = appSettings;
        _crm = crm;
        _notifyService = notifyService;
        _crmUpsertStrategy = crmUpsertStrategy;
    }


    public void Run(Guid candidateId, string json, PerformContext context)
    {
        // Abort early if CRM integration is paused (e.g., for maintenance or incident recovery).
        if (_appSettings.IsCrmIntegrationPaused)
        {
            throw new InvalidOperationException($"UpsertContactCreationChannelsJob - Aborting (CRM integration paused).");
        }

        _logger.LogInformation("UpsertContactCreationChannelsJob - Started ({Attempt})", AttemptInfo(context, _contextAdapter));
        _logger.LogInformation("UpsertContactCreationChannelsJob - Payload {Payload}", Redactor.RedactJson(json));

        
        // If this is the last retry attempt, handle fallback gracefully.
        if (IsLastAttempt(context, _contextAdapter))
        {
            HandleFinalRetry(candidateId);
            _logger.LogInformation("UpsertContactCreationChannelsJob - Deleted");
            return;
        }
        
        IEnumerable<ContactChannelCreation> contactChannelCreations = json.DeserializeChangeTracked<IEnumerable<ContactChannelCreation>>();

        HandleContactChannelsUpsert(contactChannelCreations, context);
    }


    private void HandleFinalRetry(Guid candidateId)
    {
        var candidate = _crm.GetCandidate(candidateId);

        if (candidate == null)
            return;

        var personalisation = new Dictionary<string, dynamic>();
        _notifyService.SendEmailAsync(
            candidate.Email,
            NotifyService.SignUpPartiallyFailedTemplateId,
            personalisation);
    }


    private void HandleContactChannelsUpsert(IEnumerable<ContactChannelCreation> contactChannelCreations, PerformContext context)
    {
        foreach (ContactChannelCreation contactChannelCreation in contactChannelCreations)
        {
            _crmUpsertStrategy.TryUpsert(contactChannelCreation, out string logMessage);
            _logger.LogInformation("UpsertContactCreationChannelsJob - {ModelId}: {LogMessage}",
                contactChannelCreation.Id, logMessage);
        }

        _metrics.HangfireJobQueueDuration
            .WithLabels($"UpsertContactCreationChannelsJob")
            .Observe((DateTime.UtcNow - _contextAdapter.GetJobCreatedAt(context)).TotalSeconds);
    }
}