using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using System;

namespace GetIntoTeachingApi.Models.Crm
{
    [SwaggerIgnore]
    [Entity("dfe_candidatequalification")]
    public class CandidateQualification : BaseModel, IHasCandidateId
    {
        public enum DegreeType
        {
            Degree = 222750000,
            DegreeEquivalent = 222750005,
        }

        [EntityField("dfe_contactid", typeof(EntityReference), "contact")]
        public Guid CandidateId { get; set; }
        [EntityField("dfe_type", typeof(OptionSetValue))]
        public int? TypeId { get; set; }
        [EntityField("dfe_ukdegreegrade", typeof(OptionSetValue))]
        public int? UkDegreeGradeId { get; set; }
        [EntityField("dfe_degreestatus", typeof(OptionSetValue))]
        public int? DegreeStatusId { get; set; }
        [EntityField("dfe_subject")]
        public string DegreeSubject { get; set; }
        [EntityField("createdon")]
        public DateTime? CreatedAt { get; set; }
        [EntityField("dfe_graduationyear")]
        public DateTime? GraduationYear { get; set; }
        
        [EntityField("dfe_country")]
        public Guid? DegreeCountry { get; set; }

        public CandidateQualification()
            : base()
        {
        }

        public CandidateQualification(Entity entity, ICrmService crm, IServiceProvider serviceProvider)
            : base(entity, crm, serviceProvider)
        {
        }
    }
}