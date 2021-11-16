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

        [Fact]
        public void ToEntity_WhenAlreadyRegisteredForEvent_DoesNotCreateTeachingEventRegistrationEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();
            var registration = new TeachingEventRegistration() { CandidateId = Guid.NewGuid(), EventId = Guid.NewGuid() };

            mockCrm.Setup(m => m.CandidateYetToRegisterForTeachingEvent(registration.CandidateId, registration.EventId)).Returns(false);

            var entity = registration.ToEntity(mockCrm.Object, context);

            entity.Should().BeNull();
            mockService.Verify(m => m.NewEntity("msevtmgt_eventregistration", context), Times.Never);
        }

        [Fact]
        public void ToEntity_WhenNotAlreadyRegisteredForEvent_CreatesATeachingEventRegistrationEntity()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();
            var registration = new TeachingEventRegistration() { CandidateId = Guid.NewGuid(), EventId = Guid.NewGuid() };

            mockCrm.Setup(m => m.NewEntity("msevtmgt_eventregistration", context)).Returns(new Entity("msevtmgt_eventregistration"));
            mockCrm.Setup(m => m.CandidateYetToRegisterForTeachingEvent(registration.CandidateId, registration.EventId)).Returns(true);

            registration.ToEntity(mockCrm.Object, context);

            mockCrm.Verify(m => m.NewEntity("msevtmgt_eventregistration", context), Times.Once);
        }
    }
}
