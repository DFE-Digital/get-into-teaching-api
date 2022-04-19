using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
{
    [SwaggerIgnore]
    [Entity("dfe_applyinterview")]
    public class ApplicationInterview : BaseModel
    {
        [EntityField("dfe_applyapplicationchoice", typeof(EntityReference), "dfe_applyapplicationchoice")]
        public Guid ApplicationChoiceId { get; set; }
        [EntityField("dfe_interviewid")]
        public string FindApplyId { get; set; }
        [EntityField("dfe_createdon")]
        public DateTime CreatedAt { get; set; }
        [EntityField("dfe_modifiedon")]
        public DateTime UpdatedAt { get; set; }
        [EntityField("dfe_interviewscheduledat")]
        public DateTime ScheduledAt { get; set; }
        [EntityField("dfe_interviewcancelledat")]
        public DateTime CancelledAt { get; set; }
        [EntityField("dfe_name")]
        public string Name
        {
            get
            {
                return $"Application Interview {FindApplyId}";
            }
            set
            {
                // BaseModel requires a setter for all mapped
                // attributes, although we treat this one as read-only.
            }
        }

        public ApplicationInterview()
            : base()
        {
        }

        public ApplicationInterview(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
            : base(entity, crm, validatorFactory)
        {
        }
    }
}
