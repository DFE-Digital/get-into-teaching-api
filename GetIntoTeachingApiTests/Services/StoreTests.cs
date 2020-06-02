using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class StoreTests : DatabaseTests
    {
        private static readonly Guid FindEventGuid = new Guid("ff927e43-5650-44aa-859a-8297139b8eee");
        private readonly IStore _store;

        public StoreTests()
        {
            _store = new Store(DbContext);
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

            DbContext.TeachingEvents.Count().Should().Be(5);
            DbContext.TeachingEventBuildings.Count().Should().Be(4);
        }

        [Fact]
        public void Sync_UpdatesExistingTeachingEvents()
        {
            var updatedTeachingEvents = SeedMockTeachingEvents().ToList();
            updatedTeachingEvents.ForEach(te =>
            {
                te.Name += "Updated";
                if (te.Building != null) te.Building.AddressLine1 += "Updated";
            });
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetTeachingEvents()).Returns(updatedTeachingEvents);

            _store.Sync(mockCrm.Object);

            var teachingEvents = DbContext.TeachingEvents.Include(te => te.Building).ToList();
            teachingEvents.Select(te => te.Name).ToList().ForEach(name => name.Should().Contain("Updated"));
            teachingEvents.Where(te => te.Building != null).Select(te => te.Building.AddressLine1).ToList()
                .ForEach(line1 => line1.Should().Contain("Updated"));

            DbContext.TeachingEvents.Count().Should().Be(5);
            DbContext.TeachingEventBuildings.Count().Should().Be(4);
        }

        [Fact]
        public void Sync_PopulatesBuildingCoordinates()
        {
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetTeachingEvents()).Returns(MockTeachingEvents);

            _store.Sync(mockCrm.Object);

            var teachingEvent = DbContext.TeachingEvents.Include(te => te.Building).First(te => te.Id == FindEventGuid);
            teachingEvent.Building.Coordinate.Should().Be(new Point(new Coordinate(-3.3587, 56.02748)));
        }

        [Fact]
        public void SearchTeachingEvents_WithoutFilters_ReturnsAll()
        {
            SeedMockTeachingEvents();
            var request = new TeachingEventSearchRequest() {};

            var result = _store.SearchTeachingEvents(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(
                new string[] { "Event 2", "Event 4", "Event 1", "Event 3", "Event 5" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void SearchTeachingEvents_FilteredByType_ReturnsMatching()
        {
            SeedMockTeachingEvents();
            var request = new TeachingEventSearchRequest() { TypeId = 123 };

            var result = _store.SearchTeachingEvents(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 2", "Event 4" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void SearchTeachingEvents_FilteredByStartAfter_ReturnsMatching()
        {
            SeedMockTeachingEvents();
            var request = new TeachingEventSearchRequest() { StartAfter = DateTime.Now.AddDays(6) };

            var result = _store.SearchTeachingEvents(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 3", "Event 5" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void SearchTeachingEvents_FilteredByStartBefore_ReturnsMatching()
        {
            SeedMockTeachingEvents();
            var request = new TeachingEventSearchRequest() { StartBefore = DateTime.Now.AddDays(6) };

            var result = _store.SearchTeachingEvents(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 2", "Event 4", "Event 1" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void SearchTeachingEvents_FilteredByRadius_ReturnsMatching()
        {
            SeedMockTeachingEvents();
            var request = new TeachingEventSearchRequest() { Postcode = "KY6 2NJ", Radius = 50 };

            var result = _store.SearchTeachingEvents(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 2", "Event 3" },
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

        [Theory]
        [InlineData("KY11 9YU")]
        [InlineData("ky11 9yu")]
        [InlineData("ky119yu")]
        [InlineData("k y 119 YU")]
        [InlineData("CA4 8LE")]
        public void IsValidPostcode_WithValidPostcode_ReturnsTrue(string postcode)
        {
            _store.IsValidPostcode(postcode).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("KY11 9ZZ")]
        [InlineData("KY11 9HFF")]
        [InlineData("Non-Geographic")]
        public void IsValidPostcode_WithInvalidPostcode_ReturnsFalse(string postcode)
        {
            _store.IsValidPostcode(postcode).Should().BeFalse();
        }

        private static IEnumerable<TeachingEvent> MockTeachingEvents()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: DbConfiguration.Srid);

            var event1 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                Name = "Event 1",
                StartAt = DateTime.Now.AddDays(5),
                Building = new TeachingEventBuilding()
                {
                    Id = Guid.NewGuid(),
                    AddressLine1 = "Line 1"
                }
            };

            var event2 = new TeachingEvent()
            {
                Id = FindEventGuid,
                Name = "Event 2",
                StartAt = DateTime.Now.AddDays(1),
                TypeId = 123,
                Building = new TeachingEventBuilding()
                {
                    Id = Guid.NewGuid(),
                    AddressLine1 = "Line 1",
                    AddressPostcode = "KY11 9YU",
                    Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.3587, 56.02748)),
                }
            };

            var event3 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                Name = "Event 3",
                StartAt = DateTime.Now.AddDays(10),
                Building = new TeachingEventBuilding()
                {
                    Id = Guid.NewGuid(),
                    AddressLine1 = "Line 1",
                    AddressPostcode = "KY6 2NJ",
                    Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.178240, 56.182790)),
                }
            };

            var event4 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                Name = "Event 4",
                StartAt = DateTime.Now.AddDays(3),
                TypeId = 123,
                Building = new TeachingEventBuilding()
                {
                    Id = Guid.NewGuid(),
                    AddressLine1 = "Line 1",
                    AddressPostcode = "CA4 8LE",
                    Coordinate = geometryFactory.CreatePoint(new Coordinate(-2.839040, 54.888770)),
                }
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
