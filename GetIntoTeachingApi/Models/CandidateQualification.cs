using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "dfe_candidatequalification")]
    public class CandidateQualification : BaseModel
    {
        [EntityField(Name = "dfe_category", Type = typeof(OptionSetValue))]
        public int? CategoryId { get; set; }
        [EntityField(Name = "dfe_type", Type = typeof(OptionSetValue))]
        public int? TypeId { get; set; }
        [EntityField(Name = "dfe_degreestatus", Type = typeof(OptionSetValue))]
        public int? DegreeStatusId { get; set; }

        public CandidateQualification() : base() { }

        public CandidateQualification(Entity entity, IOrganizationServiceAdapter service) : base(entity, service) { }
    }
}