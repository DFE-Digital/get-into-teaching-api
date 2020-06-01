using System;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
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
    }
}
