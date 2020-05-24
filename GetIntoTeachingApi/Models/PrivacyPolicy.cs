using System;
using System.ComponentModel.DataAnnotations.Schema;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "dfe_privacypolicy")]
    [Table("dfe_privacypolicies")]
    public class PrivacyPolicy : BaseModel
    {
        public enum Types { Web = 222750001 }

        [Column("dfe_privacypolicyid")]
        public new Guid? Id { get => base.Id; set => base.Id = value; }

        [EntityField(Name = "dfe_details")]
        [Column("dfe_details")]
        public string Text { get; set; }
        [Column("dfe_policytype")]
        public int Type { get; set; }
        [Column("dfe_active")]
        public bool IsActive { get; set; }
        [Column("createdon")]
        public DateTime CreatedAt { get; set; }

        public PrivacyPolicy() : base() { }

        public PrivacyPolicy(Entity entity, ICrmService crm) : base(entity, crm) { }
    }
}
