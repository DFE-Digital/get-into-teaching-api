using System;
using System.Text.Json.Serialization;

namespace GetIntoTeachingApi.Models
{
    public class TeachingEventAddAttendeeRequest
    {
        public Guid? CandidateId { get; set; }
        public Guid EventId { get; set; }
        public Guid AcceptedPolicyId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressPostcode { get; set; }
        public string Telephone { get; set; }
        public bool SubscribeToEvents { get; set; }
        public bool SubscribeToMailingList { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();

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
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = AcceptedPolicyId },
                ChannelId = CandidateId == null ? (int?)Candidate.Channel.Event : null,
                EligibilityRulesPassed = "false",
                OptOutOfSms = false,
                DoNotBulkEmail = true,
                DoNotEmail = false,
                DoNotBulkPostalMail = true,
                DoNotPostalMail = true,
                DoNotSendMm = true,
            };

            candidate.TeachingEventRegistrations.Add(new TeachingEventRegistration()
            {
                EventId = EventId,
                ChannelId = (int)TeachingEventRegistration.Channel.Event,
            });

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
