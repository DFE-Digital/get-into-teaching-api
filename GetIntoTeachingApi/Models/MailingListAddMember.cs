using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class MailingListAddMember : AddCandidate
    {
        [SwaggerSchema(WriteOnly = true)]
        public int? ChannelId { get; set; }

        [JsonIgnore]
        public override Candidate Candidate => CreateCandidate();

        public MailingListAddMember()
        {
        }

        public MailingListAddMember(Candidate candidate)
        {
            PopulateWithCandidate(candidate);
        }

        protected override Candidate CreateCandidate()
        {
            var candidate = base.CreateCandidate();
            candidate.ChannelId = CandidateId == null ? ChannelId ?? (int?)Candidate.Channel.MailingList : null;

            ConfigureConsent(candidate);
            AddQualification(candidate);
            AcceptPrivacyPolicy(candidate);
            ConfigureSubscriptions(candidate);

            return candidate;
        }

        protected override void ConfigureConsent(Candidate candidate)
        {
            base.ConfigureConsent(candidate);

            candidate.DoNotBulkPostalMail = true;
            candidate.DoNotPostalMail = true;
            candidate.DoNotSendMm = false;
            candidate.EligibilityRulesPassed = "false";
        }

        private void ConfigureSubscriptions(Candidate candidate)
        {
            ConfigureMailingListSubscriptions(candidate);
            candidate.MailingListSubscriptionChannelId = ChannelId ?? (int)Candidate.SubscriptionChannel.MailingList;

            if (!string.IsNullOrWhiteSpace(AddressPostcode))
            {
                ConfigureEventsSubscriptions(candidate);
                candidate.EventsSubscriptionTypeId = (int)Candidate.SubscriptionType.LocalEvent;
                candidate.EventsSubscriptionChannelId = ChannelId ?? (int)Candidate.SubscriptionChannel.Events;
            }
        }
    }
}
