using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
{
    [SwaggerIgnore]
    [Entity("dfe_applyreference")]
    public class ApplicationReference : BaseModel
    {
        [EntityField("dfe_applyapplicationform", typeof(EntityReference), "dfe_applyapplicationform")]
        public Guid ApplicationFormId { get; set; }
        [EntityField("dfe_referencefeedbackstatus", typeof(OptionSetValue))]
        public int? FeedbackStatusId { get; set; }
        [EntityField("dfe_referenceid")]
        public string FindApplyId { get; set; }
        [EntityField("dfe_requestedat")]
        public DateTime RequestedAt { get; set; }
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

        public ApplicationReference(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
            : base(entity, crm, validatorFactory)
        {
        }
    }
}
