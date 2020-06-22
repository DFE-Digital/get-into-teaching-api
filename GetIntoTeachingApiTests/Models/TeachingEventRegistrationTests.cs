using System;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingEventRegistrationTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(TeachingEventRegistration);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "msevtmgt_eventregistration");

            type.GetProperty("EventId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "msevtmgt_eventid" && a.Type == typeof(EntityReference));
        }
        /*
        [Fact]
        public void ToEntity_WhenRegistrationAlreadyExistsForCandidate_ReturnsNull()
        {
            var mockService = new Mock<IOrganizationServiceAdapter>();
            var context = mockService.Object.Context();
            var mockCrm = new Mock<ICrmService>();

            var candidate = new Candidate() { Id = Guid.NewGuid() };
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid() };
            var registration = new TeachingEventRegistration()
            {
                CandidateId = (Guid)candidate.Id,
                EventId = (Guid)teachingEvent.Id
            };

            mockCrm.Setup(m => m.CandidateYetToRegisterForTeachingEvent((Guid)candidate.Id, (Guid)teachingEvent.Id)).Returns(false);

            var entity = registration.ToEntity(mockCrm.Object, context);

            entity.Should().BeNull();
            mockService.Verify(m => m.NewEntity("msevtmgt_eventregistration", context), Times.Never);
        }*/
    }
}
