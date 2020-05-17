using System;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "dfe_candidatepastteachingposition")]
    public class CandidatePastTeachingPosition : BaseModel
    {
        [EntityField(Name = "dfe_subjecttaught", Type = typeof(EntityReference), Reference = "dfe_teachingsubjectlist")]
        public Guid? SubjectTaughtId { get; set; }
        [EntityField(Name = "dfe_educationphase", Type = typeof(OptionSetValue))]
        public int? EducationPhaseId { get; set; }

        public CandidatePastTeachingPosition() : base() { }

        public CandidatePastTeachingPosition(Entity entity, IOrganizationServiceAdapter service) : base(entity, service) { }
    }
}
