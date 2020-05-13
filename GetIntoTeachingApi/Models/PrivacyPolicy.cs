using System;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class PrivacyPolicy
    {
        public Guid Id { get; set; }
        public string Text { get; set; }

        public PrivacyPolicy() { }

        public PrivacyPolicy(Entity entity)
        {
            Id = entity.Id;
            Text = entity.GetAttributeValue<string>("dfe_details");
        }
    }
}
