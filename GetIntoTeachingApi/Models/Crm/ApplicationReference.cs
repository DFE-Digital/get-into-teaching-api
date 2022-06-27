using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
{
    [SwaggerIgnore]
    [Entity("dfe_applyreference")]
    public class ApplicationReference : BaseModel, IHasFindApplyId
    {
        // The keys for this enum need to mirror the
        // Apply API naming so we can match them up.
        public enum FeedbackStatus
        {
            Cancelled = 222750000,
            CancelledAtEndOfCycle = 222750001,
            NotRequestedYet = 222750002,
            FeedbackRequested = 222750003,
            FeedbackProvided = 222750004,
            FeedbackRefused = 222750005,
            EmailBounced = 222750006,
        }

        [EntityField("dfe_applyapplicationform", typeof(EntityReference), "dfe_applyapplicationform")]
        public Guid ApplicationFormId { get; set; }
        [EntityField("dfe_referencefeedbackstatus", typeof(OptionSetValue))]
        public int? FeedbackStatusId { get; set; }
        [EntityField("dfe_referenceid")]
        public string FindApplyId { get; set; }
        [EntityField("dfe_requestedat")]
        public DateTime? RequestedAt { get; set; }
        [EntityField("dfe_createdon")]
        public DateTime CreatedAt { get; set; }
        [EntityField("dfe_modifiedon")]
        public DateTime UpdatedAt { get; set; }
        [EntityField("dfe_referencetype")]
        public string Type { get; set; }
        [EntityField("dfe_name")]
        public string Name
        {
            get
            {
                return $"Application Reference {FindApplyId}";
            }
            set
            {
                // BaseModel requires a setter for all mapped
                // attributes, although we treat this one as read-only.
            }
        }

        public ApplicationReference()
            : base()
        {
        }

        public ApplicationReference(Entity entity, ICrmService crm, IValidator<ApplicationReference> validator)
            : base(entity, crm, validator)
        {
        }
    }
}
