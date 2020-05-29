using System.Collections.Generic;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Hangfire.Server;

namespace GetIntoTeachingApi.Jobs
{
    public class TeachingEventRegistrationJob : BaseJob
    {
        private readonly ICrmService _crm;
        private readonly INotifyService _notifyService;
        private readonly IPerformContextAdapter _contextAdapter;

        public TeachingEventRegistrationJob(ICrmService crm, INotifyService notifyService,
            IPerformContextAdapter contextAdapter)
        {
            _crm = crm;
            _notifyService = notifyService;
            _contextAdapter = contextAdapter;
        }

        public void Run(TeachingEventRegistration registration, PerformContext context)
        {
            if (IsLastAttempt(context, _contextAdapter))
            {
                var personalisation = new Dictionary<string, dynamic>();
                _notifyService.SendEmail(
                    registration.CandidateEmail,
                    NotifyService.TeachingEventRegistrationFailedTemplateId, 
                    personalisation);
            }
            else
                _crm.Save(registration);
        }
    }
}