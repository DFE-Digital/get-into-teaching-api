﻿using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
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

        public PrivacyPolicy(Entity entity, ICrmService crm, IServiceProvider serviceProvider)
            : base(entity, crm, serviceProvider)
        {
        }
    }
}
