using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class SubscriptionTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(Subscription);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_servicesubscription");

            type.GetProperty("TypeId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_servicesubscriptiontype" && a.Type == typeof(OptionSetValue));
            type.GetProperty("StatusId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "statecode" && a.Type == typeof(OptionSetValue));

            type.GetProperty("StartAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_servicesubscriptionstartdate");
            type.GetProperty("DoNotBulkEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotbulkemail");
            type.GetProperty("DoNotBulkPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotbulkpostalmail");
            type.GetProperty("DoNotEmail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotemail");
            type.GetProperty("DoNotPostalMail").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotpostalmail");
            type.GetProperty("DoNotSendMm").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "donotsendmm");
            type.GetProperty("OptOutOfSms").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_optoutsms");
            type.GetProperty("Description").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_name");
        }

        [Fact]
        public void StatusId_DefaultValue_IsActive()
        {
            var subscription = new Subscription();
            subscription.StatusId.Should().Be((int)Subscription.SubscriptionStatus.Active);
        }

        [Fact]
        public void StartAt_DefaultValue_IsNow()
        {
            var subscription = new Subscription();
            subscription.StartAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(20));
        }

        [Fact]
        public void DoNotBulkEmail_DefaultValue_IsFalse()
        {
            var subscription = new Subscription();
            subscription.DoNotBulkEmail.Should().BeFalse();
        }

        [Fact]
        public void DoNotBulkPostalMail_DefaultValue_IsFalse()
        {
            var subscription = new Subscription();
            subscription.DoNotBulkPostalMail.Should().BeFalse();
        }

        [Fact]
        public void Description_DefaultValue_IsCorrect()
        {
            new Subscription().Description.Should().Be("Online subscription created via GITIS website");
        }

        [Fact]
        public void DoNotEmail_DefaultValue_IsFalse()
        {
            var subscription = new Subscription();
            subscription.DoNotEmail.Should().BeFalse();
        }

        [Fact]
        public void DoNotPostalMail_DefaultValue_IsFalse()
        {
            var subscription = new Subscription();
            subscription.DoNotPostalMail.Should().BeFalse();
        }

        [Fact]
        public void DoNotSendMm_DefaultValue_IsFalse()
        {
            var subscription = new Subscription();
            subscription.DoNotSendMm.Should().BeFalse();
        }

        [Fact]
        public void OptOutOfSms_DefaultValue_IsFalse()
        {
            var subscription = new Subscription();
            subscription.OptOutOfSms.Should().BeFalse();
        }
    }
}
