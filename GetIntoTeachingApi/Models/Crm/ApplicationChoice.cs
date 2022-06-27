using System;
using System.Collections.Generic;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
{
    [SwaggerIgnore]
    [Entity("dfe_applyapplicationchoice")]
    public class ApplicationChoice : BaseModel, IHasFindApplyId
    {
        // The keys for this enum need to mirror the
        // Apply API naming so we can match them up.
        public enum Status
        {
            Unsubmitted = 222750000,
            Cancelled = 222750001,
            AwaitingProviderDecision = 222750002,
            Interviewing = 222750003,
            Offer = 222750004,
            PendingConditions = 222750005,
            Recruited = 222750006,
            Rejected = 222750007,
            ApplicationNotSent = 222750008,
            OfferWithdrawn = 222750009,
            Declined = 222750010,
            Withdrawn = 222750011,
            ConditionsNotMet = 222750012,
            OfferDeferred = 222750013,
        }

        [EntityField("dfe_applyapplicationform", typeof(EntityReference), "dfe_applyapplicationform")]
        public Guid ApplicationFormId { get; set; }
        [EntityField("dfe_applicationchoicestatus", typeof(OptionSetValue))]
        public int? StatusId { get; set; }
        [EntityField("dfe_applicationchoiceid")]
        public string FindApplyId { get; set; }
        [EntityField("dfe_createdon")]
        public DateTime CreatedAt { get; set; }
        [EntityField("dfe_modifiedon")]
        public DateTime UpdatedAt { get; set; }
        [EntityField("dfe_applicationchoicecoursename")]
        public string CourseName { get; set; }
        [EntityField("dfe_applicationchoicecourseuuid")]
        public string CourseId { get; set; }
        [EntityField("dfe_applicationchoiceprovider")]
        public string Provider { get; set; }
        [EntityField("dfe_name")]
        public string Name
        {
            get
            {
                return $"Application Choice {FindApplyId}";
            }
            set
            {
                // BaseModel requires a setter for all mapped
                // attributes, although we treat this one as read-only.
            }
        }

        [EntityRelationship("dfe_applyapplicationform_dfe_applyapplicationinterview_applyapplicationform", typeof(ApplicationInterview))]
        public List<ApplicationInterview> Interviews { get; set; } = new List<ApplicationInterview>();

        public ApplicationChoice()
            : base()
        {
        }

        public ApplicationChoice(Entity entity, ICrmService crm, IValidator<ApplicationChoice> validator)
            : base(entity, crm, validator)
        {
        }
    }
}
