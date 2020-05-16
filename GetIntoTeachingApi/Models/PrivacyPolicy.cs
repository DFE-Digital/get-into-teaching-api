using System;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class PrivacyPolicy : BaseModel
    {
        [Entity(Name = "dfe_details")]
        public string Text { get; set; }

        public PrivacyPolicy() : base() { }

        public PrivacyPolicy(Entity entity) : base(entity) { }
    }
}
