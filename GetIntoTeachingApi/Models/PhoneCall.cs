using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity("phonecall")]
    public class PhoneCall : BaseModel
    {
        public enum Channel
        {
            CallbackRequest = 222750003,
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

        public PhoneCall()
            : base()
        {
        }

        public PhoneCall(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
            : base(entity, crm, validatorFactory)
        {
        }
    }
}
