using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeachingEventTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(TeachingEvent);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "msevtmgt_event");

            type.GetProperty("TypeId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_event_type" && a.Type == typeof(OptionSetValue));

            type.GetProperty("Name").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_name");
            type.GetProperty("Description").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_description");
            type.GetProperty("StartAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_eventstartdate");
            type.GetProperty("EndAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "msevtmgt_eventenddate");
        }
    }
}
