using System.ComponentModel.DataAnnotations;

namespace GetIntoTeachingApi.Models
{
    public class CandidateIntermediateKeys
    {
        [Key]
        public int IntermediateCandidateId { get; set; }
        public int CrmCandidateId { get; set; }
    }
}
