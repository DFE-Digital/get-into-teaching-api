using System;
using FluentAssertions;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using Xunit;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching
{
    public class TeachingEventUpsertOperationTests
    {
        [Fact]
        public void Constructor_WithTeachingEvent()
        {
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid(), ReadableId = "test-1" };

            var operation = new TeachingEventUpsertOperation(teachingEvent);

            operation.Id.Should().Be(teachingEvent.Id);
            operation.ReadableId.Should().Be(teachingEvent.ReadableId);
        }
    }
}
