using System;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class TeachingEventAddAttendee : AddCandidate
    {
        [SwaggerSchema(WriteOnly = true)]
        public Guid? EventId { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public bool SubscribeToEvents => AddressPostcode != null && SubscribeToMailingList;
        [SwaggerSchema(WriteOnly = true)]
        public bool SubscribeToMailingList { get; set; }

        [JsonIgnore]
        public override Candidate Candidate => CreateCandidate();

        public TeachingEventAddAttendee()
        {
        }

        public TeachingEventAddAttendee(Candidate candidate)
        {
            PopulateWithCandidate(candidate);
        }

        protected override Candidate CreateCandidate()
        {
            var candidate = base.CreateCandidate();
            candidate.ChannelId = CandidateId == null ? (int?)Candidate.Channel.Event : null;

            ConfigureConsent(candidate);
            AddQualification(candidate);
            AcceptPrivacyPolicy(candidate);
            ConfigureSubscriptions(candidate);
            AddTeachingEventRegistration(candidate);

            return candidate;
        }

        protected override void ConfigureConsent(Candidate candidate)
        {
            base.ConfigureConsent(candidate);

            candidate.DoNotBulkPostalMail = !SubscribeToMailingList;
            candidate.DoNotPostalMail = !SubscribeToMailingList;
            candidate.DoNotSendMm = !SubscribeToEvents && !SubscribeToMailingList;
        }

        private void ConfigureSubscriptions(Candidate candidate)
        {
            ConfigureEventsSubscriptions(candidate);

            candidate.EventsSubscriptionChannelId = (int)Candidate.SubscriptionChannel.Events;

            candidate.EventsSubscriptionTypeId = string.IsNullOrWhiteSpace(AddressPostcode)
                ? (int)Candidate.SubscriptionType.SingleEvent
                : (int)Candidate.SubscriptionType.LocalEvent;

            if (SubscribeToMailingList)
            {
                ConfigureMailingListSubscriptions(candidate);
                candidate.MailingListSubscriptionChannelId = (int)Candidate.SubscriptionChannel.MailingList;
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
    }
}
