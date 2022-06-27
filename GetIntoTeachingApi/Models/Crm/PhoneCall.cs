using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
{
    [SwaggerIgnore]
    [Entity("phonecall")]
    public class PhoneCall : BaseModel, IHasCandidateId
    {
        public enum Channel
        {
            CallbackRequest = 222750003,
            WebsiteCallbackRequest = 222750004,
        }

        public enum Destination
        {
            Uk = 222750000,
            International = 222750001,
        }

        [EntityField("dfe_tocontactguid")]
        public string CandidateId { get; set; }
        [EntityField("dfe_channelcreation", typeof(OptionSetValue))]
        public int? ChannelId { get; set; }
        [EntityField("dfe_destination", typeof(OptionSetValue))]
        public int? DestinationId { get; set; }
        [EntityField("scheduledstart")]
        public DateTime ScheduledAt { get; set; }
        [EntityField("phonenumber")]
        public string Telephone { get; set; }
        [EntityField("subject")]
        public string Subject { get; set; }
        [EntityField("dfe_appointmentflag")]
        public bool IsAppointment { get; set; } = false;
        [EntityField("dfe_appointmentrequired")]
        public bool AppointmentRequired { get; set; } = false;
        [EntityField("directioncode")]
        public bool IsDirectionCode { get; set; } = true;
        Guid IHasCandidateId.CandidateId { get => new Guid(CandidateId); }
        [EntityField("dfe_talkingpoints")]
        public string TalkingPoints { get; set; }

        public PhoneCall()
            : base()
        {
        }

        public PhoneCall(Entity entity, ICrmService crm, IValidator<PhoneCall> validator)
            : base(entity, crm, validator)
        {
        }
    }
}
