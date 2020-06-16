using System;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "dfe_privacypolicy")]
    public class PrivacyPolicy : BaseModel
    {
        [EntityField(Name = "dfe_details")]
        public string Text { get; set; }
        [EntityField(Name = "createdon")]
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
