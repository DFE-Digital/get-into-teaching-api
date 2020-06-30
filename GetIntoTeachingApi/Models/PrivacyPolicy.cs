using System;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity("dfe_privacypolicy")]
    public class PrivacyPolicy : BaseModel
    {
        public enum Type
        {
            Web = 222750001,
        }

        [EntityField("dfe_details")]
        public string Text { get; set; }
        [EntityField("createdon")]
        public DateTime CreatedAt { get; set; }

        public PrivacyPolicy()
            : base()
        {
        }

        public PrivacyPolicy(Entity entity, ICrmService crm)
            : base(entity, crm)
        {
        }
    }
}
