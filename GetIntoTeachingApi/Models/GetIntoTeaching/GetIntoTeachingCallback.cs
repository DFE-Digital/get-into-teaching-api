using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Text.Json.Serialization;

namespace GetIntoTeachingApi.Models.GetIntoTeaching
{
    public class GetIntoTeachingCallback : ICreateContactChannel
    {
        public Guid? CandidateId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public Guid? AcceptedPolicyId { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressTelephone { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public DateTime? PhoneCallScheduledAt { get; set; }
        public string TalkingPoints { get; set; }
        
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelSourceId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelServiceId { get; set; }
        [SwaggerSchema(WriteOnly = true)]
        public int? CreationChannelActivityId { get; set; }

        [JsonIgnore]
        public Candidate Candidate => CreateCandidate();
        [JsonIgnore]
        public IDateTimeProvider DateTimeProvider { get; set; } = new DateTimeProvider();
        
        /// <summary>
        /// Provides the default read-only contact creation channel integer value. NB: this field will be deprecated.
        /// </summary>
        public int? DefaultContactCreationChannel =>
            (int?)Candidate.Channel.GetIntoTeachingCallback;

        /// <summary>
        /// Provides the default read-only creation channel source identifier.
        /// </summary>
        public int? DefaultCreationChannelSourceId =>
            (int?)ContactChannelCreation.CreationChannelSource.GITWebsite;

        /// <summary>
        /// Provides the default read-only creation channel service identifier.
        /// </summary>
        public int? DefaultCreationChannelServiceId =>
            (int?)ContactChannelCreation.CreationChannelService.MailingList;

        /// <summary>
        /// Provides the default read-only creation channel activity identifier.
        /// </summary>
        public int? DefaultCreationChannelActivityId => null;

        public GetIntoTeachingCallback(){
        }

        public GetIntoTeachingCallback(Candidate candidate)
        {
            PopulateWithCandidate(candidate);
        }

        private void PopulateWithCandidate(Candidate candidate)
        {
            CandidateId = candidate.Id;

            Email = candidate.Email;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            AddressTelephone = candidate.AddressTelephone.StripExitCode();
        }

        private Candidate CreateCandidate()
        {
            var candidate = new Candidate()
            {
                Id = CandidateId,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                AddressTelephone = AddressTelephone,
            };
            candidate.ConfigureChannel(contactChannelCreator: this, candidateId: CandidateId);
            SchedulePhoneCall(candidate);
            AcceptPrivacyPolicy(candidate);

            return candidate;
        }

        private void SchedulePhoneCall(Candidate candidate)
        {
            if (PhoneCallScheduledAt != null)
            {
                candidate.PhoneCall = new PhoneCall()
                {
                    Telephone = candidate.AddressTelephone,
                    DestinationId = (int)PhoneCall.Destination.Uk,
                    ScheduledAt = (DateTime)PhoneCallScheduledAt,
                    ChannelId = (int)PhoneCall.Channel.WebsiteCallbackRequest,
                    Subject = $"Scheduled phone call requested by {candidate.FullName}",
                    TalkingPoints = TalkingPoints,
                };
            }
        }

        private void AcceptPrivacyPolicy(Candidate candidate)
        {
            if (AcceptedPolicyId != null)
            {
                candidate.PrivacyPolicy = new CandidatePrivacyPolicy()
                {
                    AcceptedPolicyId = (Guid)AcceptedPolicyId,
                    AcceptedAt = DateTimeProvider.UtcNow,
                };
            }
        }
    }
}
