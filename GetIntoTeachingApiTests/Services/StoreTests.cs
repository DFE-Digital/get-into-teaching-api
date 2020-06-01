using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class StoreTests : DatabaseTests
    {
        private static readonly Guid FindEventGuid = new Guid("ff927e43-5650-44aa-859a-8297139b8eee");
        private readonly IStore _store;

        public StoreTests()
        {
            var mockLocationService = new Mock<ILocationService>();
            _store = new Store(DbContext, mockLocationService.Object);
        }

        [Fact]
        public void Sync_InsertsNewTeachingEvents()
        {
            var mockTeachingEvents = MockTeachingEvents().ToList();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetTeachingEvents()).Returns(mockTeachingEvents);

            _store.Sync(mockCrm.Object);

            var teachingEventNames = DbContext.TeachingEvents.Select(teachingEvent => teachingEvent.Name);
            teachingEventNames.Should().BeEquivalentTo(mockTeachingEvents.Select(teachingEvent => teachingEvent.Name));
        }

        [Fact]
        public void Sync_UpdatesExistingTeachingEvents()
        {
            var updatedTeachingEvents = SeedMockTeachingEvents().ToList();
            updatedTeachingEvents.ForEach(teachingEvent => teachingEvent.Name += "Updated");
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetTeachingEvents()).Returns(updatedTeachingEvents);

            _store.Sync(mockCrm.Object);

            var teachingEventNames = DbContext.TeachingEvents.Select(teachingEvent => teachingEvent.Name).ToList();
            teachingEventNames.ForEach(name => name.Should().Contain("Updated"));
        }

        [Fact]
        public void SearchTeachingEvents_ReturnsMatchingEventsInOrder()
        {
            SeedMockTeachingEvents();
            var request = new TeachingEventSearchRequest() {Postcode = "CA4 8LE", TypeId = 123};

            var result = _store.SearchTeachingEvents(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] {"Event 2", "Event 4"},
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void GetTeachingEvents_ReturnsMatchingEvent()
        {
            SeedMockTeachingEvents();
            var result = _store.GetTeachingEvent(FindEventGuid);

            result.Id.Should().Be(FindEventGuid);
        }

        [Fact]
        public void GetUpcomingTeachingEvents_ReturnsUpcomingEventsInOrder()
        {
            SeedMockTeachingEvents();
            var result = _store.GetUpcomingTeachingEvents(3);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] {"Event 2", "Event 4", "Event 1"},
                options => options.WithStrictOrdering());
        }

        private static IEnumerable<TeachingEvent> MockTeachingEvents()
        {
            var event1 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                Name = "Event 1",
                StartAt = DateTime.Now.AddDays(5),
            };

            var event2 = new TeachingEvent()
            {
                Id = FindEventGuid,
                Name = "Event 2",
                StartAt = DateTime.Now.AddDays(1),
                TypeId = 123,
            };

            var event3 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                Name = "Event 3",
                StartAt = DateTime.Now.AddDays(10),
            };

            var event4 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                Name = "Event 4",
                StartAt = DateTime.Now.AddDays(3),
                TypeId = 123,
            };

            var event5 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                Name = "Event 5",
                StartAt = DateTime.Now.AddDays(15),
            };

            return new TeachingEvent[] {event1, event2, event3, event4, event5};
        }

        private IEnumerable<TeachingEvent> SeedMockTeachingEvents()
        {
            var teachingEvents = MockTeachingEvents().ToList();

            DbContext.AddRange(teachingEvents);
            DbContext.SaveChanges();

            return teachingEvents;
        }
    }
}
