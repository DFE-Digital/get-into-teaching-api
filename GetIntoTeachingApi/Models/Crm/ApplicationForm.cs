using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
{
    [SwaggerIgnore]
    [Entity("dfe_applyapplicationform")]
    public class ApplicationForm : BaseModel, IHasCandidateId
    {
        [EntityField("dfe_contact", typeof(EntityReference), "contact")]
        public Guid CandidateId { get; set; }
        [EntityField("dfe_candidateapplyphase", typeof(OptionSetValue))]
        public int? PhaseId { get; set; }
        [EntityField("dfe_applicationformid")]
        public string FindApplyId { get; set; }
        [EntityField("dfe_createdon")]
        public DateTime CreatedAt { get; set; }
        [EntityField("dfe_modifiedon")]
        public DateTime UpdatedAt { get; set; }
        [EntityField("dfe_name")]
        public string Name
        {
            get
            {
                return $"Application Form {FindApplyId}";
            }
            set
            {
                // BaseModel requires a setter for all mapped
                // attributes, although we treat this one as read-only.
            }
        }

        public ApplicationForm()
            : base()
        {
        }

        public ApplicationForm(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
            : base(entity, crm, validatorFactory)
        {
        }
    }
}
