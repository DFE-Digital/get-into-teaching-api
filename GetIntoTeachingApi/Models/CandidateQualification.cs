using System;

namespace GetIntoTeachingApi.Models
{
    public class CandidateQualification
    {
        public Guid? Id { get; set; }
        public int? CategoryId { get; set; }
        public int? TypeId { get; set; }
        public int? DegreeStatusId { get; set; }
    }
}