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

        public Entity PopulateEntity(Entity entity)
        {
            if (SubjectTaughtId != null) entity["dfe_subjecttaught"] = new EntityReference("dfe_teachingsubjectlist", (Guid)SubjectTaughtId);
            if (EducationPhaseId != null) entity["dfe_educationphase"] = new OptionSetValue((int)EducationPhaseId);

            return entity;
        }
    }
}
