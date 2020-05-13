using System;

namespace GetIntoTeachingApi.Models
{
    public class CandidatePastTeachingPosition
    {
        public Guid? Id { get; set; }
        public Guid? SubjectTaughtId { get; set; }
        public int? EducationPhaseId { get; set; }
    }
}
