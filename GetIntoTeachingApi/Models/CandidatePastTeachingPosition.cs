using System;
using System.ComponentModel.DataAnnotations.Schema;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "dfe_candidatepastteachingposition")]
    [Table("dfe_candidatepastteachingpositions")]
    public class CandidatePastTeachingPosition : BaseModel
    {
        [Column("dfe_candidatepastteachingpositionid")]
        public new Guid? Id { get => base.Id; set => base.Id = value; }

        [EntityField(Name = "dfe_subjecttaught", Type = typeof(EntityReference), Reference = "dfe_teachingsubjectlist")]
        [Column("dfe_subjecttaught")]
        public Guid? SubjectTaughtId { get; set; }
        [EntityField(Name = "dfe_educationphase", Type = typeof(OptionSetValue))]
        [Column("dfe_educationphase")]
        public int? EducationPhaseId { get; set; }

        public CandidatePastTeachingPosition() : base() { }

        public CandidatePastTeachingPosition(Entity entity, ICrmService crm) : base(entity, crm) { }
    }
}
