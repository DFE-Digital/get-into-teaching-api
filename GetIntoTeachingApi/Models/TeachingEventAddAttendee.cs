using System;
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

            AlreadySubscribedToMailingList = candidate.HasActiveSubscriptionOfType(Subscription.ServiceType.MailingList);
            AlreadySubscribedToEvents = candidate.HasActiveSubscriptionOfType(Subscription.ServiceType.Event);
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
            };

            AddTeachingEventRegistration(candidate);
            AcceptPrivacyPolicy(candidate);
            AddSubscriptions(candidate);
            ConfigureConsent(candidate);

            return candidate;
        }

        private void ConfigureConsent(Candidate candidate)
        {
            candidate.OptOutOfSms = false;
            candidate.DoNotBulkEmail = !SubscribeToEvents && !SubscribeToMailingList;
            candidate.DoNotBulkPostalMail = !SubscribeToMailingList;
            candidate.DoNotEmail = false;
            candidate.DoNotPostalMail = !SubscribeToMailingList;
            candidate.DoNotSendMm = !SubscribeToEvents && !SubscribeToMailingList;
        }

        private void AddSubscriptions(Candidate candidate)
        {
            candidate.Subscriptions.Add(new Subscription()
            {
                TypeId = (int)Subscription.ServiceType.Event,
                DoNotBulkPostalMail = true,
                DoNotPostalMail = true,
                DoNotBulkEmail = !SubscribeToEvents,
                DoNotSendMm = !SubscribeToEvents,
            });

            if (SubscribeToMailingList)
            {
                candidate.Subscriptions.Add(new Subscription()
                {
                    TypeId = (int)Subscription.ServiceType.MailingList,
                    DoNotBulkPostalMail = true,
                    DoNotPostalMail = true,
                });
            }
        }

        private void AddTeachingEventRegistration(Candidate candidate)
        {
            if (EventId != null)
            {
                candidate.TeachingEventRegistrations.Add(new TeachingEventRegistration()
                {
                    EventId = (Guid)EventId,
                    ChannelId = (int)TeachingEventRegistration.Channel.Event,
                    IsCancelled = false,
                    RegistrationNotificationSeen = false,
                });
            }
        }

        private void AcceptPrivacyPolicy(Candidate candidate)
        {
            if (AcceptedPolicyId != null)
            {
                candidate.PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = (Guid)AcceptedPolicyId };
            }
        }
    }
}
