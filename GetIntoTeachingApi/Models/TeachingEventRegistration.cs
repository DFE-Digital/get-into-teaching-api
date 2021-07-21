using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity("msevtmgt_eventregistration")]
    [SwaggerIgnore]
    public class TeachingEventRegistration : BaseModel, IHasCandidateId
    {
        public enum Channel
        {
            Event = 222750003,
        }

        [EntityField("msevtmgt_contactid", typeof(EntityReference), "contact")]
        public Guid CandidateId { get; set; }
        [EntityField("msevtmgt_eventid", typeof(EntityReference), "msevtmgt_event")]
        public Guid EventId { get; set; }
        [EntityField("dfe_channelcreation", typeof(OptionSetValue))]
        public int? ChannelId { get; set; }
        [EntityField("msevtmgt_iscanceled")]
        public bool? IsCancelled { get; set; }
        [EntityField("msevtmgt_registrationnotificationseen")]
        public bool? RegistrationNotificationSeen { get; set; }

        public TeachingEventRegistration()
            : base()
        {
        }

        public TeachingEventRegistration(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
            : base(entity, crm, validatorFactory)
        {
        }

        protected override bool ShouldMap(ICrmService crm)
        {
            var alreadyRegistered = !crm.CandidateYetToRegisterForTeachingEvent(CandidateId, EventId);

            if (alreadyRegistered)
            {
                return false;
            }

            return base.ShouldMap(crm);
        }
    }
}