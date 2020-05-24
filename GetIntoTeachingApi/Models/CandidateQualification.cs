using System;
using System.ComponentModel.DataAnnotations.Schema;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "dfe_candidatequalification")]
    [Table("dfe_candidatequalifications")]
    public class CandidateQualification : BaseModel
    {
        [Column("dfe_candidatequalificationid")]
        public new Guid? Id { get => base.Id; set => base.Id = value; }

        [EntityField(Name = "dfe_category", Type = typeof(OptionSetValue))]
        [Column("dfe_category")]
        public int? CategoryId { get; set; }
        [EntityField(Name = "dfe_type", Type = typeof(OptionSetValue))]
        [Column("dfe_type")]
        public int? TypeId { get; set; }
        [EntityField(Name = "dfe_degreestatus", Type = typeof(OptionSetValue))]
        [Column("dfe_degreestatus")]
        public int? DegreeStatusId { get; set; }

        public CandidateQualification() : base() { }

        public CandidateQualification(Entity entity, ICrmService crm) : base(entity, crm) { }
    }
}