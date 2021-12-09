using System;
using System.Text.Json.Serialization;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
{
    [Entity("dfe_candidateschoolexperience")]
    public class CandidateSchoolExperience : BaseModel, IHasCandidateId
    {
        public enum SchoolExperienceStatus
        {
            Requested = 1, // default
            Confirmed = 222750000,
            Withdrawn = 222750001,
            Rejected = 222750002,
            CancelledBySchool = 222750003,
            CancelledByCandidate = 222750004,
            Completed = 222750005,
        }

        [JsonIgnore]
        [EntityField("dfe_contactid", typeof(EntityReference), "contact")]
        public Guid CandidateId { get; set; }
        [EntityField("dfe_urn")]
        public string SchoolUrn { get; set; }
        [EntityField("dfe_placementduration")]
        public int? DurationOfPlacementInDays { get; set; }
        [EntityField("dfe_dateofschoolexperience")]
        public DateTime? DateOfSchoolExperience { get; set; }
        [EntityField("statuscode", typeof(OptionSetValue))]
        public int? Status { get; set; }
        [EntityField("dfe_teachingsubject", typeof(EntityReference), "dfe_teachingsubjectlist")]
        public Guid? TeachingSubjectId { get; set; }
        [EntityField("dfe_notes")]
        public string Notes { get; set; }
        [EntityField("dfe_where")]
        public string SchoolName { get; set; }

        public CandidateSchoolExperience()
            : base()
        {
        }

        public CandidateSchoolExperience(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
            : base(entity, crm, validatorFactory)
        {
        }
    }
}
