using System;
using System.ComponentModel.DataAnnotations.Schema;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Table("dfe_candidatepastteachingpositions")]
    public class CandidatePastTeachingPosition : BaseModel
    {
        [Column("dfe_candidatepastteachingpositionid")]
        public new Guid? Id { get => base.Id; set => base.Id = value; }

        [Column("dfe_SubjectTaught")]
        public TeachingSubject SubjectTaught { get; set; }
        [Column("dfe_educationphase")]
        public int? EducationPhaseId { get; set; }

        public CandidatePastTeachingPosition() : base() { }

        public CandidatePastTeachingPosition(Entity entity, ICrmService crm) : base(entity, crm) { }
    }
}
