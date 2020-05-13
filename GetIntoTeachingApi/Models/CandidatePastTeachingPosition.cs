using System;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class CandidatePastTeachingPosition
    {
        public Guid? Id { get; set; }
        public Guid? SubjectTaughtId { get; set; }
        public int? EducationPhaseId { get; set; }

        public CandidatePastTeachingPosition() { }

        public CandidatePastTeachingPosition(Entity entity)
        {
            Id = entity.Id;
            SubjectTaughtId = entity.GetAttributeValue<EntityReference>("dfe_subjecttaught")?.Id;
            EducationPhaseId = entity.GetAttributeValue<OptionSetValue>("dfe_educationphase")?.Value;
        }

        public Entity ToEntity()
        {
            var entity = new Entity("dfe_candidatepastteachingposition");

            if (Id != null) entity.Id = (Guid)Id;

            if (SubjectTaughtId != null)
            {
                entity.Attributes.Add("dfe_subjecttaught",
                    new EntityReference("dfe_teachingsubjectlist", (Guid)SubjectTaughtId));
            }

            if (EducationPhaseId != null)
            {
                entity.Attributes.Add("dfe_educationphase", new OptionSetValue((int)EducationPhaseId));
            }

            return entity;
        }
    }
}
