using System;
using System.Collections.Generic;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
{
    [SwaggerIgnore]
    [Entity("dfe_applyapplicationform")]
    public class ApplicationForm : BaseModel, IHasCandidateId, IHasFindApplyId
    {
        // The keys for this enum need to mirror the
        // Apply API naming so we can match them up.
        public enum Status
        {
            NeverSignedIn = 222750000,
            UnsubmittedNotStartedForm = 222750001,
            UnsubmittedInProgress = 222750002,
            AwaitingProviderDecisions = 222750003,
            AwaitingCandidateResponse = 222750004,
            Recruited = 222750005,
            PendingConditions = 222750006,
            OfferDeferred = 222750007,
            EndedWithoutSuccess = 222750008,
            UnknownState = 222750009,
        }

        // The keys for this enum need to mirror the
        // Apply API naming so we can match them up.
        public enum Phase
        {
            Apply1 = 222750000,
            Apply2 = 222750001,
        }

        public enum RecruitmentCycleYear
        {
            Year2020 = 222750000,
            Year2021 = 222750001,
            Year2022 = 222750002,
        }

        [EntityField("dfe_contact", typeof(EntityReference), "contact")]
        public Guid CandidateId { get; set; }
        [EntityField("dfe_applyphase", typeof(OptionSetValue))]
        public int? PhaseId { get; set; }
        [EntityField("dfe_applystatus", typeof(OptionSetValue))]
        public int? StatusId { get; set; }
        [EntityField("dfe_recruitmentyear", typeof(OptionSetValue))]
        public int? RecruitmentCycleYearId { get; set; }
        [EntityField("dfe_applicationformid")]
        public string FindApplyId { get; set; }
        [EntityField("dfe_createdon")]
        public DateTime CreatedAt { get; set; }
        [EntityField("dfe_modifiedon")]
        public DateTime UpdatedAt { get; set; }
        [EntityField("dfe_submittedatdate")]
        public DateTime? SubmittedAt { get; set; }
        [EntityField("dfe_qualificationscompleted", null, null, new[] { "APPLY_API_V1_2" })]
        public bool? QualificationsCompleted { get; set; }
        [EntityField("dfe_referencescompleted", null, null, new[] { "APPLY_API_V1_2" })]
        public bool? ReferencesCompleted { get; set; }
        [EntityField("dfe_applicationchoicescompleted", null, null, new[] { "APPLY_API_V1_2" })]
        public bool? ApplicationChoicesCompleted { get; set; }
        [EntityField("dfe_personalstatementcompleted", null, null, new[] { "APPLY_API_V1_2" })]
        public bool? PersonalStatementCompleted { get; set; }
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

        [EntityRelationship("dfe_applyapplicationform_dfe_applyapplicationchoice_applyapplicationform", typeof(ApplicationChoice))]
        public List<ApplicationChoice> Choices { get; set; } = new List<ApplicationChoice>();
        [EntityRelationship("dfe_applyapplicationform_dfe_applyreference_applyapplicationform", typeof(ApplicationReference))]
        public List<ApplicationReference> References { get; set; } = new List<ApplicationReference>();

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
