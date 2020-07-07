using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
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
    }
}
