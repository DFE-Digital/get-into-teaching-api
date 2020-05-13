using System;

namespace GetIntoTeachingApi.Models
{
    public class Candidate
    {
        public Guid? Id { get; set; }
        public Guid? PreferredTeachingSubjectId { get; set; }
        public int? PreferredEducationPhaseId { get; set; }
        public int? LocationId { get; set; }
        public int? InitialTeacherTrainingYearId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Address Address { get; set; }
        public CandidateQualification[] Qualifications { get; set; }
        public CandidatePastTeachingPosition[] PastTeachingPositions { get; set; }
    }
}
