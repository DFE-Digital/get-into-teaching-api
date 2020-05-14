using System;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class CandidateQualification
    {
        public Guid? Id { get; set; }
        public int? CategoryId { get; set; }
        public int? TypeId { get; set; }
        public int? DegreeStatusId { get; set; }

        public CandidateQualification() {}

        public CandidateQualification(Entity entity)
        {
            Id = entity.Id;
            CategoryId = entity.GetAttributeValue<OptionSetValue>("dfe_category")?.Value;
            TypeId = entity.GetAttributeValue<OptionSetValue>("dfe_type")?.Value;
            DegreeStatusId = entity.GetAttributeValue<OptionSetValue>("dfe_degreestatus")?.Value;
        }

        public Entity ToEntity()
        {
            var entity = new Entity("dfe_candidatequalification");

            if (Id != null) entity.Id = (Guid)Id;
            if (CategoryId != null) entity.Attributes.Add("dfe_category", new OptionSetValue((int)CategoryId));
            if (TypeId != null) entity.Attributes.Add("dfe_type", new OptionSetValue((int)TypeId));
            if (DegreeStatusId != null) entity.Attributes.Add("dfe_degreestatus", new OptionSetValue((int)DegreeStatusId));

            return entity;
        }
    }
}