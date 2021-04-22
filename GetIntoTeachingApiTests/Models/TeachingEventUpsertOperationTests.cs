using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
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
