using System;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class CandidateQualification : BaseModel
    {
        [Entity(Name = "dfe_category", Type = typeof(OptionSetValue))]
        public int? CategoryId { get; set; }
        [Entity(Name = "dfe_type", Type = typeof(OptionSetValue))]
        public int? TypeId { get; set; }
        [Entity(Name = "dfe_degreestatus", Type = typeof(OptionSetValue))]
        public int? DegreeStatusId { get; set; }

        public CandidateQualification() : base() { }

        public CandidateQualification(Entity entity) : base(entity) { }
    }
}