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
    public class MailingListAddMemberJob : BaseJob
    {
        private readonly ICrmService _crm;
        private readonly INotifyService _notifyService;
        private readonly IPerformContextAdapter _contextAdapter;
        private readonly ILogger<MailingListAddMemberJob> _logger;

        public MailingListAddMemberJob(
            IEnv env,
            ICrmService crm,
            INotifyService notifyService,
            IPerformContextAdapter contextAdapter,
            ILogger<MailingListAddMemberJob> logger)
            : base(env)
        {
            _crm = crm;
            _logger = logger;
            _notifyService = notifyService;
            _contextAdapter = contextAdapter;
        }

        public void Run(MailingListAddMemberRequest request, PerformContext context)
        {
            _logger.LogInformation($"MailingListAddMemberJob - Started ({AttemptInfo(context, _contextAdapter)})");

            if (IsLastAttempt(context, _contextAdapter))
            {
                NotifyCandidateOfFailure(request);
                _logger.LogInformation("MailingListAddMemberJob - Deleted");
            }
            else
            {
                AddMemberToMailingList(request);
                _logger.LogInformation("MailingListAddMemberJob - Succeeded");
            }
        }

        private void AddMemberToMailingList(MailingListAddMemberRequest request)
        {
            var candidate = FindOrCreateCandidate(request.CandidateId);

            AddServiceSubscription(candidate);
            UpdateCandidateDetails(candidate, request);
        }

        private void UpdateCandidateDetails(Candidate candidate, MailingListAddMemberRequest request)
        {
            candidate.PreferredTeachingSubjectId = request.PreferredTeachingSubjectId;
            candidate.Email = request.Email;
            candidate.FirstName = request.FirstName;
            candidate.LastName = request.LastName;
            candidate.Telephone = request.Telephone ?? candidate.Telephone;
            candidate.AddressPostcode = request.AddressPostcode;
            candidate.PrivacyPolicy = request.PrivacyPolicy;

            _crm.Save(candidate);
        }

        private void NotifyCandidateOfFailure(MailingListAddMemberRequest request)
        {
            // We fire and forget the email, ensuring the job succeeds.
            _notifyService.SendEmailAsync(
                request.Email,
                NotifyService.MailingListAddMemberFailedEmailTemplateId,
                new Dictionary<string, dynamic>());
        }

        private Candidate FindOrCreateCandidate(Guid? candidateId)
        {
            return candidateId != null ? _crm.GetCandidate((Guid)candidateId) : new Candidate();
        }

        private void AddServiceSubscription(Candidate candidate)
        {
            var alreadySubscribed = candidate.Id != null && _crm.IsCandidateSubscribedToServiceOfType(
                (Guid)candidate.Id, (int)ServiceSubscription.ServiceType.MailingList);

            if (alreadySubscribed)
            {
                return;
            }

            var subscription = new ServiceSubscription()
            {
                TypeId = (int)ServiceSubscription.ServiceType.MailingList,
            };

            candidate.ServiceSubscriptions.Add(subscription);
        }
    }
}