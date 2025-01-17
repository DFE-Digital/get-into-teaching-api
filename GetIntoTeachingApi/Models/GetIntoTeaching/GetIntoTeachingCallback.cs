﻿using System;
using System.Linq;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models.GetIntoTeaching
{
    public class GetIntoTeachingCallback
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

        public GetIntoTeachingCallback()
        {
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

            ConfigureChannel(candidate);
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

        private void ConfigureChannel(Candidate candidate)
        {
            if (CandidateId == null)
            {
                if (CreationChannelSourceId.HasValue)
                {
                    candidate.ChannelId = null;
                    // NB: CreationChannel should be true only if it is the first ContactChannelCreation record
                    AddCandidateCreationChannel(candidate, !candidate.ContactChannelCreations.Any());
                }
                else
                {
                    candidate.ChannelId = (int?)Candidate.Channel.GetIntoTeachingCallback;
                }
            } 
            else // Candidate record already exists 
            {
                // NB: we do not update a candidate's ChannelId for an existing record
                // NB: CreationChannel should always be false for existing candidates
                if (CreationChannelSourceId.HasValue)
                {
                    AddCandidateCreationChannel(candidate, false);
                }
            }
        }
        
        private void AddCandidateCreationChannel(Candidate candidate, bool creationChannel)
        {
            candidate.ContactChannelCreations.Add(new ContactChannelCreation()
            {
                CreationChannel = creationChannel,
                CreationChannelSourceId = CreationChannelSourceId,
                CreationChannelServiceId = CreationChannelServiceId,
                CreationChannelActivityId = CreationChannelActivityId,
            });
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
