using System;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity("dfe_candidatequalification")]
    public class CandidateQualification : BaseModel
    {
        [EntityField("dfe_ukdegreegrade", typeof(OptionSetValue))]
        public int? UkDegreeGradeId { get; set; }
        [EntityField("dfe_degreestatus", typeof(OptionSetValue))]
        public int? DegreeStatusId { get; set; }
        [EntityField("dfe_subject")]
        public string Subject { get; set; }

        public CandidateQualification()
            : base()
        {
        }

        public CandidateQualification(Entity entity, ICrmService crm)
            : base(entity, crm)
        {
        }
    }
}