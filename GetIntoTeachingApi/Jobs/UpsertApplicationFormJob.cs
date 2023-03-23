﻿using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace GetIntoTeachingApi.Jobs
{
    public class UpsertApplicationFormJob : BaseJob
    {
        private readonly IPerformContextAdapter _contextAdapter;
        private readonly IMetricService _metrics;
        private readonly ICrmService _crm;
        private readonly IAppSettings _appSettings;
        private readonly ILogger<UpsertApplicationFormJob> _logger;

        public UpsertApplicationFormJob(
            IEnv env,
            IRedisService redis,
            IPerformContextAdapter contextAdapter,
            IMetricService metrics,
            ICrmService crm,
            ILogger<UpsertApplicationFormJob> logger,
            IAppSettings appSettings)
            : base(env, redis)
        {
            _contextAdapter = contextAdapter;
            _metrics = metrics;
            _crm = crm;
            _logger = logger;
            _appSettings = appSettings;
        }

        public void Run(string json, PerformContext context)
        {
            //var form = json.DeserializeChangeTracked<ApplicationForm>();

            //if (_appSettings.IsCrmIntegrationPaused)
            //{
            //    throw new InvalidOperationException("UpsertApplicationFormJob - Aborting (CRM integration paused).");
            //}

            //_logger.LogInformation("UpsertApplicationFormJob - Started ({Attempt})", AttemptInfo(context, _contextAdapter));
            //_logger.LogInformation("UpsertApplicationFormJob - Payload {Payload}", Redactor.RedactJson(json));

            //if (IsLastAttempt(context, _contextAdapter))
            //{
            //    _logger.LogInformation("UpsertApplicationFormJob - Deleted");
            //}
            //else
            //{
            //    SaveApplicationForm(form);
            //    SaveApplicationReferences(form.References, (Guid)form.Id);
            //    SaveApplicationChoices(form.Choices, (Guid)form.Id);

            //    _logger.LogInformation("UpsertApplicationFormJob - Succeeded - {Id}", form.Id);
            //}

            //var duration = (DateTime.UtcNow - _contextAdapter.GetJobCreatedAt(context)).TotalSeconds;
            //_metrics.HangfireJobQueueDuration.WithLabels("UpsertApplicationFormJob").Observe(duration);
        }

        private void SaveApplicationForm(ApplicationForm form)
        {
            var existing = _crm.GetApplyModels<ApplicationForm>(new string[] { form.ApplyId }).FirstOrDefault();

            if (existing != null)
            {
                form.Id = existing.Id;
            }

            _crm.Save(form);
        }

        private void SaveApplicationChoices(IEnumerable<ApplicationChoice> choices, Guid formId)
        {
            var existing = _crm.GetApplyModels<ApplicationChoice>(choices.Select(c => c.ApplyId));

            choices?.ForEach(c =>
            {
                c.Id = existing.FirstOrDefault(e => e.ApplyId == c.ApplyId)?.Id;
                c.ApplicationFormId = formId;
                _crm.Save(c);

                SaveApplicationInterviews(c.Interviews, (Guid)c.Id);
            });
        }

        private void SaveApplicationReferences(IEnumerable<ApplicationReference> references, Guid formId)
        {
            var existing = _crm.GetApplyModels<ApplicationReference>(references.Select(r => r.ApplyId));

            references?.ForEach(r =>
            {
                r.Id = existing.FirstOrDefault(e => e.ApplyId == r.ApplyId)?.Id;
                r.ApplicationFormId = formId;
                _crm.Save(r);
            });
        }

        private void SaveApplicationInterviews(IEnumerable<ApplicationInterview> interviews, Guid choiceId)
        {
            var existing = _crm.GetApplyModels<ApplicationInterview>(interviews.Select(i => i.ApplyId));

            interviews?.ForEach(i =>
            {
                i.Id = existing.FirstOrDefault(e => e.ApplyId == i.ApplyId)?.Id;
                i.ApplicationChoiceId = choiceId;
                _crm.Save(i);
            });
        }
    }
}
