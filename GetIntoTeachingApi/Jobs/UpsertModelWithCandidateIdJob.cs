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

/// <summary>
/// Hangfire job responsible for executing upsert logic on CRM-tracked models.
/// Applies conditional routing and error-tolerant final attempt behavior for ContactChannelCreation.
/// </summary>
public class UpsertModelWithCandidateIdJob<T> : BaseJob
    where T : BaseModel, IHasCandidateId
{
    private readonly IPerformContextAdapter _contextAdapter;
    private readonly IMetricService _metrics;
    private readonly IAppSettings _appSettings;
    private readonly INotifyService _notifyService;
    private readonly ICrmService _crm;
    private readonly ILogger<UpsertModelWithCandidateIdJob<T>> _logger;
    private readonly ICrmlUpsertStrategy<ContactChannelCreation> _crmUpsertStrategy;

    /// <summary>
    /// Injects service dependencies for CRM, notifications, metrics, and upsert dispatch.
    /// </summary>
    public UpsertModelWithCandidateIdJob(
        IEnv env,
        IRedisService redis,
        IPerformContextAdapter contextAdapter,
        ICrmService crm,
        IMetricService metrics,
        ILogger<UpsertModelWithCandidateIdJob<T>> logger,
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

    /// <summary>
    /// Executes the job using the supplied JSON payload and Hangfire context.
    /// Applies CRM save logic or strategy routing depending on model type and retry state.
    /// </summary>
    public void Run(string json, PerformContext context)
    {
        var typeName = typeof(T).Name;

        // Abort early if CRM integration is paused (e.g., for maintenance or incident recovery).
        if (_appSettings.IsCrmIntegrationPaused)
        {
            throw new InvalidOperationException($"UpsertModelJob<{typeName}> - Aborting (CRM integration paused).");
        }

        _logger.LogInformation("UpsertModelJob<{TypeName}> - Started ({Attempt})", typeName, AttemptInfo(context, _contextAdapter));
        _logger.LogInformation("UpsertModelJob<{TypeName}> - Payload {Payload}", typeName, Redactor.RedactJson(json));

        var model = json.DeserializeChangeTracked<T>();

        // If this is the last retry attempt, handle fallback gracefully.
        if (IsLastAttempt(context, _contextAdapter))
        {
            HandleFinalRetry(model);
            _logger.LogInformation("UpsertModelJob<{TypeName}> - Deleted", typeName);
            return;
        }

        // Route by model type using pattern matching.
        switch (model)
        {
            case ContactChannelCreation creationChannel:
                HandleContactChannelUpsert(creationChannel, typeName, context);
                break;

            default:
                _crm.Save(model);
                _logger.LogInformation("UpsertModelJob<{TypeName}> - Succeeded - {Id}", typeName, model.Id);
                break;
        }
    }

    /// <summary>
    /// Handles terminal job attempt failure by notifying the candidate of partial CRM save.
    /// </summary>
    private void HandleFinalRetry(T model)
    {
        var candidate = _crm.GetCandidate(model.CandidateId);

        if (candidate == null)
            return;

        var personalisation = new Dictionary<string, dynamic>();
        _notifyService.SendEmailAsync(
            candidate.Email,
            NotifyService.SignUpPartiallyFailedTemplateId,
            personalisation);
    }

    /// <summary>
    /// Applies domain-specific upsert strategy to ContactChannelCreation entities.
    /// Logs diagnostic metadata and captures queue latency metrics.
    /// </summary>
    private void HandleContactChannelUpsert(ContactChannelCreation creationChannel, string typeName, PerformContext context)
    {
        _crmUpsertStrategy.TryUpsert(creationChannel, out string logMessage);

        _logger.LogInformation("UpsertModelJob<{TypeName}> - {ModelId}: {LogMessage}",
            typeName, creationChannel.Id, logMessage);

        _metrics.HangfireJobQueueDuration
            .WithLabels($"UpsertModelJob<{typeName}>")
            .Observe((DateTime.UtcNow - _contextAdapter.GetJobCreatedAt(context)).TotalSeconds);
    }
}