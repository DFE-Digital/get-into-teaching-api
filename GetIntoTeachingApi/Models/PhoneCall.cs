using System;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity("phonecall")]
    public class PhoneCall : BaseModel
    {
        [EntityField("dfe_channelcreation", typeof(OptionSetValue))]
        public int? ChannelId { get; set; }
        [EntityField("scheduledstart")]
        public DateTime ScheduledAt { get; set; }
        [JsonIgnore]
        [EntityField("phonenumber")]
        public string Telephone { get; set; }
        [JsonIgnore]
        [EntityField("subject")]
        public string Subject { get; set; }
        [JsonIgnore]
        [EntityField("dfe_appointmentflag")]
        public bool IsAppointment { get; set; } = false;
        [JsonIgnore]
        [EntityField("dfe_appointmentrequired")]
        public bool AppointmentRequired { get; set; } = false;
        [JsonIgnore]
        [EntityField("directioncode")]
        public bool IsDirectionCode { get; set; } = true;

        public PhoneCall()
            : base()
        {
        }

        public PhoneCall(Entity entity, ICrmService crm)
            : base(entity, crm)
        {
        }

        public void PopulateWithCandidate(Candidate candidate)
        {
            Telephone = candidate.Telephone;
            Subject = $"Scheduled phone call requested by {candidate.FullName}";
        }
    }
}
