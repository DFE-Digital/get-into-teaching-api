﻿using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "dfe_privacypolicy")]
    public class PrivacyPolicy : BaseModel
    {
        [EntityField(Name = "dfe_details")]
        public string Text { get; set; }

        public PrivacyPolicy() : base() { }

        public PrivacyPolicy(Entity entity, IOrganizationServiceAdapter service) : base(entity, service) { }
    }
}
