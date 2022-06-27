using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
{
    [SwaggerIgnore]
    [Entity("dfe_candidatepastteachingposition")]
    public class CandidatePastTeachingPosition : BaseModel, IHasCandidateId
    {
        public enum EducationPhase
        {
            Secondary = 222750001,
        }

        [EntityField("dfe_contactid", typeof(EntityReference), "contact")]
        public Guid CandidateId { get; set; }
        [EntityField("dfe_subjecttaught", typeof(EntityReference), "dfe_teachingsubjectlist")]
        public Guid? SubjectTaughtId { get; set; }
        [EntityField("dfe_educationphase", typeof(OptionSetValue))]
        public int? EducationPhaseId { get; set; }
        [EntityField("createdon")]
        public DateTime? CreatedAt { get; set; }

        public CandidatePastTeachingPosition()
            : base()
        {
        }

        public CandidatePastTeachingPosition(Entity entity, ICrmService crm, IValidator<CandidatePastTeachingPosition> validator)
            : base(entity, crm, validator)
        {
        }
    }
}
