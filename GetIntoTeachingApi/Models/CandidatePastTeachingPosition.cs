using System;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class CandidatePastTeachingPosition : BaseModel
    {
        [Entity(Name = "dfe_subjecttaught", Type = typeof(EntityReference), Reference = "dfe_teachingsubjectlist")]
        public Guid? SubjectTaughtId { get; set; }
        [Entity(Name = "dfe_educationphase", Type = typeof(OptionSetValue))]
        public int? EducationPhaseId { get; set; }

        public CandidatePastTeachingPosition() : base() { }

        public CandidatePastTeachingPosition(Entity entity) : base(entity) { }
    }
}
