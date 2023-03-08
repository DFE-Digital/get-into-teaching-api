using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Moq;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm
{
    public class TeachingEventRegistrationTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(TeachingEventRegistration);
            type.Should().BeDecoratedWith<SwaggerIgnoreAttribute>();

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "msevtmgt_eventregistration");

            type.GetProperty("CandidateId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "msevtmgt_contactid" && a.Type == typeof(EntityReference) && a.Reference == "contact");
            type.GetProperty("EventId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "msevtmgt_eventid" && a.Type == typeof(EntityReference) && a.Reference == "msevtmgt_event");

            type.GetProperty("ChannelId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_channelcreation" && a.Type == typeof(OptionSetValue));

            type.GetProperty("IsCancelled").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "msevtmgt_iscanceled");
            type.GetProperty("RegistrationNotificationSeen").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "msevtmgt_registrationnotificationseen");
        }
    }
}
