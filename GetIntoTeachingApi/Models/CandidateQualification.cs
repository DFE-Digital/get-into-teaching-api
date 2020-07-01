using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity("dfe_candidatequalification")]
    public class CandidateQualification : BaseModel
    {
        [EntityField("dfe_category", typeof(OptionSetValue))]
        public int? CategoryId { get; set; }
        [EntityField("dfe_type", typeof(OptionSetValue))]
        public int? TypeId { get; set; }
        [EntityField("dfe_degreestatus", typeof(OptionSetValue))]
        public int? DegreeStatusId { get; set; }

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