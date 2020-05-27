using System;
using System.ComponentModel.DataAnnotations.Schema;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Table("dfe_candidatequalifications")]
    public class CandidateQualification : BaseModel
    {
        [Column("dfe_candidatequalificationid")]
        public new Guid? Id { get => base.Id; set => base.Id = value; }

        [Column("dfe_category")]
        public int? CategoryId { get; set; }
        [Column("dfe_type")]
        public int? TypeId { get; set; }
        [Column("dfe_degreestatus")]
        public int? DegreeStatusId { get; set; }

        public CandidateQualification() : base() { }

        public CandidateQualification(Entity entity, ICrmService crm) : base(entity, crm) { }
    }
}