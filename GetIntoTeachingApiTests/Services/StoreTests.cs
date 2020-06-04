using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
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

            var ids = DbContext.TeachingEvents.Select(te => te.Id);
            ids.Should().BeEquivalentTo(mockTeachingEvents.Select(te => te.Id));
            DbContext.TeachingEvents.Count().Should().Be(6);
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
            DbContext.TeachingEvents.Count().Should().Be(6);
            DbContext.TeachingEventBuildings.Count().Should().Be(4);
        }

        [Fact]
        public void Sync_PopulatesTeachingEventBuildingCoordinates()
        {
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetTeachingEvents()).Returns(MockTeachingEvents);

            _store.Sync(mockCrm.Object);

            var teachingEvent = DbContext.TeachingEvents.Include(te => te.Building).First(te => te.Id == FindEventGuid);
            teachingEvent.Building.Coordinate.Should().Be(new Point(new Coordinate(-3.3587, 56.02748)));
        }

        [Fact]
        public void Sync_InsertsNewPrivacyPolicies()
        {
            var mockPolicies = MockPrivacyPolicies().ToList();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetPrivacyPolicies()).Returns(mockPolicies);

            _store.Sync(mockCrm.Object);

            var ids = DbContext.PrivacyPolicies.Select(p => p.Id);
            ids.Should().BeEquivalentTo(mockPolicies.Select(p => p.Id));
            DbContext.PrivacyPolicies.Count().Should().Be(3);
        }

        [Fact]
        public void Sync_UpdatesExistingPrivacyPolicies()
        {
            var updatedPolicies = SeedMockPrivacyPolicies().ToList();
            updatedPolicies.ForEach(te => te.Text += "Updated");
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetPrivacyPolicies()).Returns(updatedPolicies);

            _store.Sync(mockCrm.Object);

            var policies = DbContext.PrivacyPolicies.ToList();
            policies.Select(te => te.Text).ToList().ForEach(name => name.Should().Contain("Updated"));
            DbContext.PrivacyPolicies.Count().Should().Be(3);
        }

        [Fact]
        public void GetPrivacyPolicies_ReturnsAll()
        {
            SeedMockPrivacyPolicies();

            var result = _store.GetPrivacyPolicies();

            result.Select(p => p.Text).Should().BeEquivalentTo(new string[] { "Policy 1", "Policy 2", "Policy 3" });
        }

        [Fact]
        public void GetLatestPrivacyPolicy_ReturnsMostRecent()
        {
            SeedMockPrivacyPolicies();

            var result = _store.GetLatestPrivacyPolicy();

            result.Text.Should().Be("Policy 2");
        }

        [Fact]
        public void SearchTeachingEvents_WithoutFilters_ReturnsAll()
        {
            SeedMockTeachingEvents();
            var request = new TeachingEventSearchRequest() { };

            var result = _store.SearchTeachingEvents(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(
                new string[] { "Event 2", "Event 4", "Event 1", "Event 3", "Event 5", "Event 6" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public void SearchTeachingEvents_WithFilters_ReturnsMatching()
        {
            SeedMockTeachingEvents();
            var request = new TeachingEventSearchRequest()
            {
                Postcode = "KY6 2NJ", 
                Radius = 15, 
                TypeId = 123, 
                StartAfter = DateTime.Now, 
                StartBefore = DateTime.Now.AddDays(3)
            };

            var result = _store.SearchTeachingEvents(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(
                new string[] { "Event 2" },
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

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 3", "Event 5", "Event 6" },
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
            var request = new TeachingEventSearchRequest() { Postcode = "KY6 2NJ", Radius = 15 };

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
            var sharedBuildingId = Guid.NewGuid();

            var event1 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                Name = "Event 1",
                StartAt = DateTime.Now.AddDays(5),
                Building = new TeachingEventBuilding()
                {
                    Id = sharedBuildingId,
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
                }
            };

            var event5 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                Name = "Event 5",
                StartAt = DateTime.Now.AddDays(15),
            };

            var event6 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                Name = "Event 6",
                StartAt = DateTime.Now.AddDays(60),
                Building = new TeachingEventBuilding()
                {
                    Id = sharedBuildingId,
                    AddressLine1 = "Line 1"
                }
            };

            return new TeachingEvent[] { event1, event2, event3, event4, event5, event6 };
        }

        private IEnumerable<TeachingEvent> SeedMockTeachingEvents()
        {
            var teachingEvents = MockTeachingEvents().ToList();
            var mockCrm = new Mock<ICrmService>();

            mockCrm.Setup(m => m.GetTeachingEvents()).Returns(teachingEvents);
            
            _store.Sync(mockCrm.Object);

            return teachingEvents;
        }

        private static IEnumerable<PrivacyPolicy> MockPrivacyPolicies()
        {
            var policy1 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Policy 1", CreatedAt = DateTime.Now.AddDays(-10) };
            var policy2 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Policy 2", CreatedAt = DateTime.Now };
            var policy3 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Policy 3", CreatedAt = DateTime.Now.AddDays(-5) };

            return new PrivacyPolicy[] {policy1, policy2, policy3};
        }

        private IEnumerable<PrivacyPolicy> SeedMockPrivacyPolicies()
        {
            var privacyPolicies = MockPrivacyPolicies().ToList();
            var mockCrm = new Mock<ICrmService>();

            mockCrm.Setup(m => m.GetPrivacyPolicies()).Returns(privacyPolicies);

            _store.Sync(mockCrm.Object);

            return privacyPolicies;
        }
    }
}
