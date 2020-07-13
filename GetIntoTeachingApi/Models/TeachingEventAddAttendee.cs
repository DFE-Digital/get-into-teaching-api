using System;
using System.Linq;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class TeachingEventAddAttendee
    {
        public Guid? CandidateId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? EventId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressPostcode { get; set; }
        public string Telephone { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public bool SubscribeToEvents { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public bool SubscribeToMailingList { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToEvents { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public bool AlreadySubscribedToMailingList { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();

        public TeachingEventAddAttendee()
        {
        }

        public TeachingEventAddAttendee(Candidate candidate)
        {
            PopulateWithCandidate(candidate);
        }

        private void PopulateWithCandidate(Candidate candidate)
        {
            CandidateId = candidate.Id;

            Email = candidate.Email;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            AddressPostcode = candidate.AddressPostcode;
            Telephone = candidate.Telephone;

            AlreadySubscribedToMailingList = candidate.HasActiveSubscriptionToService(Subscription.ServiceType.MailingList);
            AlreadySubscribedToEvents = candidate.HasActiveSubscriptionToService(Subscription.ServiceType.Event);
        }

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                AddressPostcode = AddressPostcode,
                Telephone = Telephone,
                ChannelId = CandidateId == null ? (int?)Candidate.Channel.Event : null,
                EligibilityRulesPassed = "false",
                OptOutOfSms = false,
                DoNotBulkEmail = true,
                DoNotEmail = false,
                DoNotBulkPostalMail = true,
                DoNotPostalMail = true,
                DoNotSendMm = true,
            };

            if (EventId != null)
            {
                candidate.TeachingEventRegistrations.Add(new TeachingEventRegistration()
                {
                    EventId = (Guid)EventId,
                    ChannelId = (int)TeachingEventRegistration.Channel.Event,
                });
            }

            if (AcceptedPolicyId != null)
            {
                candidate.PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = (Guid)AcceptedPolicyId };
            }

            if (SubscribeToEvents)
            {
                candidate.Subscriptions.Add(new Subscription() { TypeId = (int)Subscription.ServiceType.Event });
            }

            if (SubscribeToMailingList)
            {
                candidate.Subscriptions.Add(new Subscription() { TypeId = (int)Subscription.ServiceType.MailingList });
            }

            return candidate;
        }
    }
}
