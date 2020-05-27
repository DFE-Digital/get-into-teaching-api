using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetIntoTeachingApi.Models
{
    [Table("dfe_candidateprivacypolicy")]
    public class CandidatePrivacyPolicy : BaseModel
    {
        [Column("dfe_candidateprivacypolicyid")]
        public new Guid? Id { get => base.Id; set => base.Id = value; }

        [Column("dfe_PrivacyPolicyNumber")]
        public PrivacyPolicy AcceptedPolicy { get; set; }

        public CandidatePrivacyPolicy() : base() { }
    }
}
