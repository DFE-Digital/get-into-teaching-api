using System;
using System.ComponentModel.DataAnnotations;

namespace GetIntoTeachingApi.Models
{
    public class CandidateIntermediateKeys
    {
        [Key]
        public Guid IntermediateCandidateId { get; set; }
        public Guid CrmCandidateId { get; set; }
    }
}
