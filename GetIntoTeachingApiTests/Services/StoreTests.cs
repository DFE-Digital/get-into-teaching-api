﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Location = GetIntoTeachingApi.Models.Location;

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
        public async void CheckStatusAsync_WhenHealthy_ReturnsOk()
        {
            (await _store.CheckStatusAsync()).Should().Be(HealthCheckResponse.StatusOk);
        }

        [Fact]
        public async void CheckStatusAsync_WhenUnhealthy_ReturnsError()
        {
            await DbContext.DisposeAsync();
            (await _store.CheckStatusAsync()).Should().Contain("Cannot access a disposed object.");
        }

        [Fact]
        public async void SyncAsync_WithFailure_RetainsExistingData()
        {
            await SeedMockPrivacyPoliciesAsync();
            var countBefore = DbContext.PrivacyPolicies.Count();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetPrivacyPolicies()).Throws<Exception>();

            _store.Invoking(s => s.SyncAsync(mockCrm.Object))
                .Should().Throw<Exception>();

            var countAfter = DbContext.PrivacyPolicies.Count();
            countBefore.Should().BeGreaterThan(0);
            countAfter.Should().Be(countBefore);
        }

        [Fact]
        public async void SyncAsync_InsertsNewTeachingEvents()
        {
            var mockTeachingEvents = MockTeachingEvents().ToList();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetTeachingEvents()).Returns(mockTeachingEvents);

            await _store.SyncAsync(mockCrm.Object);

            var ids = DbContext.TeachingEvents.Select(te => te.Id);
            ids.Should().BeEquivalentTo(mockTeachingEvents.Select(te => te.Id));
            DbContext.TeachingEvents.Count().Should().Be(6);
            DbContext.TeachingEventBuildings.Count().Should().Be(4);
        }

        [Fact]
        public async void SyncAsync_UpdatesExistingTeachingEvents()
        {
            var updatedTeachingEvents = (await SeedMockTeachingEventsAsync()).ToList();
            updatedTeachingEvents.ForEach(te =>
            {
                te.Name += "Updated";
                if (te.Building != null) te.Building.AddressLine1 += "Updated";
            });
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetTeachingEvents()).Returns(updatedTeachingEvents);

            await _store.SyncAsync(mockCrm.Object);

            var teachingEvents = DbContext.TeachingEvents.Include(te => te.Building).ToList();
            teachingEvents.Select(te => te.Name).ToList().ForEach(name => name.Should().Contain("Updated"));
            teachingEvents.Where(te => te.Building != null).Select(te => te.Building.AddressLine1).ToList()
                .ForEach(line1 => line1.Should().Contain("Updated"));
            DbContext.TeachingEvents.Count().Should().Be(6);
            DbContext.TeachingEventBuildings.Count().Should().Be(4);
        }

        [Fact]
        public async void SyncAsync_DeletesOrphanedTeachingEventsAndBuildings()
        {
            var teachingEvents = (await SeedMockTeachingEventsAsync()).ToList();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetTeachingEvents()).Returns(teachingEvents.GetRange(0, 1));

            await _store.SyncAsync(mockCrm.Object);

            DbContext.TeachingEvents.Should().BeEquivalentTo(teachingEvents.GetRange(0, 1));
            DbContext.TeachingEventBuildings.Should()
                .BeEquivalentTo(teachingEvents.GetRange(0, 1).Select(te => te.Building));
        }

        [Fact]
        public async void SyncAsync_PopulatesTeachingEventBuildingCoordinates()
        {
            SeedMockLocations();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetTeachingEvents()).Returns(MockTeachingEvents);

            await _store.SyncAsync(mockCrm.Object);

            var teachingEvent = DbContext.TeachingEvents.Include(te => te.Building).First(te => te.Id == FindEventGuid);
            teachingEvent.Building.Coordinate.Should().Be(new Point(new Coordinate(-3.3587, 56.02748)));
        }

        [Fact]
        public async void SyncAsync_InsertsNewPrivacyPolicies()
        {
            var mockPolicies = MockPrivacyPolicies().ToList();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetPrivacyPolicies()).Returns(mockPolicies);

            await _store.SyncAsync(mockCrm.Object);

            var ids = DbContext.PrivacyPolicies.Select(p => p.Id);
            ids.Should().BeEquivalentTo(mockPolicies.Select(p => p.Id));
            DbContext.PrivacyPolicies.Count().Should().Be(3);
        }

        [Fact]
        public async void SyncAsync_UpdatesExistingPrivacyPolicies()
        {
            var updatedPolicies = (await SeedMockPrivacyPoliciesAsync()).ToList();
            updatedPolicies.ForEach(te => te.Text += "Updated");
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetPrivacyPolicies()).Returns(updatedPolicies);

            await _store.SyncAsync(mockCrm.Object);

            var policies = DbContext.PrivacyPolicies.ToList();
            policies.Select(te => te.Text).ToList().ForEach(name => name.Should().Contain("Updated"));
            DbContext.PrivacyPolicies.Count().Should().Be(3);
        }

        [Fact]
        public async void SyncAsync_DeletesOrphanedPrivacyPolicies()
        {
            var policies = (await SeedMockPrivacyPoliciesAsync()).ToList();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetPrivacyPolicies()).Returns(policies.GetRange(0, 2));

            await _store.SyncAsync(mockCrm.Object);

            var remainingPolicies = DbContext.PrivacyPolicies.ToArray();
            remainingPolicies.Should().BeEquivalentTo(policies.GetRange(0, 2));
        }

        [Fact]
        public async void SyncAsync_InsertsNewLookupItems()
        {
            var mockCountries = MockCountries().ToList();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetLookupItems("dfe_country")).Returns(mockCountries);

            await _store.SyncAsync(mockCrm.Object);

            var ids = DbContext.TypeEntities.Select(e => e.Id);
            ids.Should().BeEquivalentTo(mockCountries.Select(e => e.Id));
            DbContext.TypeEntities.Count().Should().Be(3);
        }

        [Fact]
        public async void SyncAsync_UpdatesExistingLookupItems()
        {
            var updatedCountries = (await SeedMockCountriesAsync()).ToList();
            updatedCountries.ForEach(c => c.Value += "Updated");
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetLookupItems("dfe_country")).Returns(updatedCountries);

            await _store.SyncAsync(mockCrm.Object);

            var countries = DbContext.TypeEntities.ToList();
            countries.Select(c => c.Value).ToList().ForEach(value => value.Should().Contain("Updated"));
            DbContext.TypeEntities.Count().Should().Be(3);
        }

        [Fact]
        public async void SyncAsync_DeletesOrphanedLookupItems()
        {
            var countries = (await SeedMockCountriesAsync()).ToList();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetLookupItems("dfe_country")).Returns(countries.GetRange(0, 2));

            await _store.SyncAsync(mockCrm.Object);

            var remainingCountries = DbContext.TypeEntities.Where(te => te.EntityName == "dfe_country").ToArray();
            remainingCountries.Should().BeEquivalentTo(countries.GetRange(0, 2));
        }

        [Fact]
        public async void SyncAsync_InsertsNewPickListItems()
        {
            var mockYears = MockInitialTeacherTrainingYears().ToList();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetPickListItems("contact", "dfe_ittyear")).Returns(mockYears);

            await _store.SyncAsync(mockCrm.Object);

            var ids = DbContext.TypeEntities.Select(e => e.Id);
            ids.Should().BeEquivalentTo(mockYears.Select(e => e.Id));
            DbContext.TypeEntities.Count().Should().Be(3);
        }

        [Fact]
        public async void Sync_UpdatesExistingPickListItems()
        {
            var updatedYears = (await SeedMockInitialTeacherTrainingYearsAsync()).ToList();
            updatedYears.ForEach(c => c.Value += "Updated");
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetPickListItems("contact", "dfe_ittyear")).Returns(updatedYears);

            await _store.SyncAsync(mockCrm.Object);

            var countries = DbContext.TypeEntities.ToList();
            countries.Select(c => c.Value).ToList().ForEach(value => value.Should().Contain("Updated"));
            DbContext.TypeEntities.Count().Should().Be(3);
        }

        [Fact]
        public async void SyncAsync_DeletesOrphanedPickListItems()
        {
            var years = (await SeedMockInitialTeacherTrainingYearsAsync()).ToList();
            var mockCrm = new Mock<ICrmService>();
            mockCrm.Setup(m => m.GetPickListItems("contact", "dfe_ittyear")).Returns(years.GetRange(0, 2));

            await _store.SyncAsync(mockCrm.Object);

            var remainingCountries = DbContext.TypeEntities
                .Where(te => te.EntityName == "contact" && te.AttributeName == "dfe_ittyear").ToArray();
            remainingCountries.Should().BeEquivalentTo(years.GetRange(0, 2));
        }

        [Fact]
        public async void SyncAsync_SyncsAllTypeEntities()
        {
            var mockCrm = new Mock<ICrmService>();

            await _store.SyncAsync(mockCrm.Object);

            mockCrm.Verify(m => m.GetLookupItems("dfe_country"));
            mockCrm.Verify(m => m.GetLookupItems("dfe_teachingsubjectlist"));
            mockCrm.Verify(m => m.GetPickListItems("contact", "dfe_ittyear"));
            mockCrm.Verify(m => m.GetPickListItems("contact", "dfe_preferrededucationphase01"));
            mockCrm.Verify(m => m.GetPickListItems("contact", "dfe_channelcreation"));
            mockCrm.Verify(m => m.GetPickListItems("contact", "dfe_hasgcseenglish"));
            mockCrm.Verify(m => m.GetPickListItems("contact", "dfe_websiteplanningretakeenglishgcse"));
            mockCrm.Verify(m => m.GetPickListItems("contact", "dfe_websitedescribeyourself"));
            mockCrm.Verify(m => m.GetPickListItems("contact", "dfe_websitewhereinconsiderationjourney"));
            mockCrm.Verify(m => m.GetPickListItems("contact", "dfe_typeofcandidate"));
            mockCrm.Verify(m => m.GetPickListItems("contact", "dfe_candidatestatus"));
            mockCrm.Verify(m => m.GetPickListItems("contact", "dfe_iscandidateeligibleforadviser"));
            mockCrm.Verify(m => m.GetPickListItems("contact", "dfe_isadvisorrequiredos"));
            mockCrm.Verify(m => m.GetPickListItems("dfe_candidatequalification", "dfe_degreestatus"));
            mockCrm.Verify(m => m.GetPickListItems("dfe_candidatequalification", "dfe_ukdegreegrade"));
            mockCrm.Verify(m => m.GetPickListItems("dfe_candidatequalification", "dfe_type"));
            mockCrm.Verify(m => m.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase"));
            mockCrm.Verify(m => m.GetPickListItems("msevtmgt_event", "dfe_event_type"));
            mockCrm.Verify(m => m.GetPickListItems("phonecall", "dfe_channelcreation"));
        }

        [Fact]
        public async void GetLookupItems_ReturnsMatching()
        {
            await SeedMockCountriesAsync();

            var result = _store.GetLookupItems("dfe_country");

            result.Select(t => t.Value).Should().BeEquivalentTo(new string[] { "Country 1", "Country 2", "Country 3" });
        }

        [Fact]
        public async void GetPickListItems_ReturnsMatching()
        {
            await SeedMockInitialTeacherTrainingYearsAsync();

            var result = _store.GetPickListItems("contact", "dfe_ittyear");

            result.Select(t => t.Value).Should().BeEquivalentTo(new string[] { "2009", "2010", "2011" });
        }

        [Fact]
        public async void GetPrivacyPolicies_ReturnsAll()
        {
            await SeedMockPrivacyPoliciesAsync();

            var result = _store.GetPrivacyPolicies();

            result.Select(t => t.Text).Should().BeEquivalentTo(new string[] { "Policy 1", "Policy 2", "Policy 3" });
        }

        [Fact]
        public async void GetLatestPrivacyPolicy_ReturnsMostRecent()
        {
            await SeedMockPrivacyPoliciesAsync();

            var result = await _store.GetLatestPrivacyPolicyAsync();

            result.Text.Should().Be("Policy 2");
        }

        [Fact]
        public async void SearchTeachingEvents_WithoutFilters_ReturnsAll()
        {
            await SeedMockTeachingEventsAsync();
            var request = new TeachingEventSearchRequest() { };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(
                new string[] { "Event 2", "Event 4", "Event 1", "Event 3", "Event 5", "Event 6" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async void SearchTeachingEvents_WithFilters_ReturnsMatching()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAsync();
            var request = new TeachingEventSearchRequest()
            {
                Postcode = "KY6 2NJ",
                Radius = 15,
                TypeId = 123,
                StartAfter = DateTime.Now,
                StartBefore = DateTime.Now.AddDays(3)
            };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(
                new string[] { "Event 2" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async void SearchTeachingEvents_FilteredByRadius_ReturnsMatching()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAsync();
            var request = new TeachingEventSearchRequest() { Postcode = "KY6 2NJ", Radius = 15 };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 2", "Event 3" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async void SearchTeachingEvents_FilteredByType_ReturnsMatching()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAsync();
            var request = new TeachingEventSearchRequest() { TypeId = 123 };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 2", "Event 4" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async void SearchTeachingEvents_FilteredByStartAfter_ReturnsMatching()
        {
            await SeedMockTeachingEventsAsync();
            var request = new TeachingEventSearchRequest() { StartAfter = DateTime.Now.AddDays(6) };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 3", "Event 5", "Event 6" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async void SearchTeachingEvents_FilteredByStartBefore_ReturnsMatching()
        {
            await SeedMockTeachingEventsAsync();
            var request = new TeachingEventSearchRequest() { StartBefore = DateTime.Now.AddDays(6) };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 2", "Event 4", "Event 1" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async void GetTeachingEvents_ReturnsMatchingEvent()
        {
            await SeedMockTeachingEventsAsync();
            var result = await _store.GetTeachingEventAsync(FindEventGuid);

            result.Id.Should().Be(FindEventGuid);
        }

        [Fact]
        public async void GetUpcomingTeachingEvents_ReturnsUpcomingEventsInOrder()
        {
            await SeedMockTeachingEventsAsync();
            var result = _store.GetUpcomingTeachingEvents(3);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 2", "Event 4", "Event 1" },
                options => options.WithStrictOrdering());
        }

        [Theory]
        [InlineData("KY11 9YU")]
        [InlineData("ky11 9yu")]
        [InlineData("ky119yu")]
        [InlineData("k y 119 YU")]
        public void IsValidPostcode_WithValidPostcode_ReturnsTrue(string postcode)
        {
            SeedMockLocations();
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
            SeedMockLocations();
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

        private async Task<IEnumerable<TeachingEvent>> SeedMockTeachingEventsAsync()
        {
            var teachingEvents = MockTeachingEvents().ToList();
            var mockCrm = new Mock<ICrmService>();

            mockCrm.Setup(m => m.GetTeachingEvents()).Returns(teachingEvents);

            await _store.SyncAsync(mockCrm.Object);

            return teachingEvents;
        }

        private static IEnumerable<PrivacyPolicy> MockPrivacyPolicies()
        {
            var policy1 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Policy 1", CreatedAt = DateTime.Now.AddDays(-10) };
            var policy2 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Policy 2", CreatedAt = DateTime.Now };
            var policy3 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Policy 3", CreatedAt = DateTime.Now.AddDays(-5) };

            return new [] { policy1, policy2, policy3 };
        }

        private async Task<IEnumerable<PrivacyPolicy>> SeedMockPrivacyPoliciesAsync()
        {
            var privacyPolicies = MockPrivacyPolicies().ToList();
            var mockCrm = new Mock<ICrmService>();

            mockCrm.Setup(m => m.GetPrivacyPolicies()).Returns(privacyPolicies);

            await _store.SyncAsync(mockCrm.Object);

            return privacyPolicies;
        }

        private static IEnumerable<TypeEntity> MockCountries()
        {
            var country1 = new TypeEntity() { Id = Guid.NewGuid().ToString(), Value = "Country 1", EntityName = "dfe_country" };
            var country2 = new TypeEntity() { Id = Guid.NewGuid().ToString(), Value = "Country 2", EntityName = "dfe_country" };
            var country3 = new TypeEntity() { Id = Guid.NewGuid().ToString(), Value = "Country 3", EntityName = "dfe_country" };

            return new TypeEntity[] { country1, country2, country3 };
        }

        private async Task<IEnumerable<TypeEntity>> SeedMockCountriesAsync()
        {
            var types = MockCountries().ToList();
            var mockCrm = new Mock<ICrmService>();

            mockCrm.Setup(m => m.GetLookupItems("dfe_country")).Returns(types);

            await _store.SyncAsync(mockCrm.Object);

            return types;
        }

        private static IEnumerable<TypeEntity> MockInitialTeacherTrainingYears()
        {
            var year1 = new TypeEntity() { Id = "1", Value = "2009", EntityName = "contact", AttributeName = "dfe_ittyear" };
            var year2 = new TypeEntity() { Id = "2", Value = "2010", EntityName = "contact", AttributeName = "dfe_ittyear" };
            var year3 = new TypeEntity() { Id = "3", Value = "2011", EntityName = "contact", AttributeName = "dfe_ittyear" };

            return new TypeEntity[] { year1, year2, year3 };
        }

        private async Task<IEnumerable<TypeEntity>> SeedMockInitialTeacherTrainingYearsAsync()
        {
            var types = MockInitialTeacherTrainingYears().ToList();
            var mockCrm = new Mock<ICrmService>();

            mockCrm.Setup(m => m.GetPickListItems("contact", "dfe_ittyear")).Returns(types);

            await _store.SyncAsync(mockCrm.Object);

            return types;
        }

        private void SeedMockLocations()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: DbConfiguration.Wgs84Srid);
            var locations = new Location[]
            {
                new Location() {Postcode = "ky119yu", Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.35870, 56.02748))},
                new Location() {Postcode = "ca48le", Coordinate = geometryFactory.CreatePoint(new Coordinate(-2.84000, 54.89014))},
                new Location() {Postcode = "ky62nj", Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.178240, 56.182790))},
                new Location() {Postcode = "kw14yl", Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.10075, 58.64102))},
                new Location() {Postcode = "tr182ab", Coordinate = geometryFactory.CreatePoint(new Coordinate(-5.53987, 50.12279))},
            };

            DbContext.Locations.AddRange(locations);
            DbContext.SaveChanges();
        }
    }
}
