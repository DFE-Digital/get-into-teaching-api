using Bogus;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Microsoft.Xrm.Sdk;
using Moq;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Location = GetIntoTeachingApi.Models.Location;
using PickListItem = GetIntoTeachingApi.Models.PickListItem;

namespace GetIntoTeachingApiTests.Services
{
    [Collection("Database")]
    public class StoreTests : DatabaseTests
    {
        private static readonly Guid FindEventGuid = new Guid("ff927e43-5650-44aa-859a-8297139b8eee");
        private readonly IStore _store;
        private readonly Mock<IGeocodeClientAdapter> _mockGeocodeClient;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IEnv> _mockEnv;

        public StoreTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            _mockGeocodeClient = new Mock<IGeocodeClientAdapter>();
            _mockCrm = new Mock<ICrmService>();
            _mockEnv = new Mock<IEnv>();
            _store = new Store(DbContext, _mockGeocodeClient.Object, _mockCrm.Object, new DateTimeProvider(), _mockEnv.Object);

            Store.ClearFailedPostcodeLookupCache();
        }

        [Fact]
        public async Task CheckStatusAsync_WhenHealthy_ReturnsOk()
        {
            (await _store.CheckStatusAsync()).Should().Be(HealthCheckResponse.StatusOk);
        }

        [Fact]
        public async Task CheckStatusAsync_WhenUnhealthy_ReturnsError()
        {
            await DbContext.DisposeAsync();
            (await _store.CheckStatusAsync()).Should().Contain("Cannot access a disposed context instance.");
        }

        [Fact]
        public async Task SyncAsync_WithFailure_RetainsExistingData()
        {
            await SeedMockPrivacyPoliciesAsync();
            var countBefore = DbContext.PrivacyPolicies.Count();
            _mockCrm.Setup(m => m.GetPrivacyPolicies()).Throws<Exception>();

            await _store.Awaiting(s => s.SyncAsync()).Should().ThrowAsync<Exception>();

            var countAfter = DbContext.PrivacyPolicies.Count();
            countBefore.Should().BeGreaterThan(0);
            countAfter.Should().Be(countBefore);
        }

        [Fact]
        public async Task SyncAsync_InsertsNewTeachingEvents()
        {
            await SeedMockTeachingEventBuildingsAsync();
            var mockTeachingEvents = MockTeachingEvents().ToList();
            _mockCrm.Setup(m => m.GetTeachingEvents(It.Is<DateTime>(d => CheckGetTeachingEventsAfterDate(d)))).Returns(mockTeachingEvents);

            await _store.SyncAsync();

            var ids = DbContext.TeachingEvents.Select(te => te.Id);
            ids.Should().BeEquivalentTo(mockTeachingEvents.Select(te => te.Id));
            DbContext.TeachingEvents.Count().Should().Be(8);
        }

        [Fact]
        public async Task SyncAsync_UpdatesExistingTeachingEvents()
        {
            var updatedTeachingEvents = (await SeedMockTeachingEventsAndBuildingsAsync()).ToList();
            updatedTeachingEvents.ForEach(te =>
            {
                te.Name += "Updated";
                if (te.Building != null) te.Building.AddressLine1 += "Updated";
            });
            _mockCrm.Setup(m => m.GetTeachingEvents(It.Is<DateTime>(d => CheckGetTeachingEventsAfterDate(d)))).Returns(updatedTeachingEvents);

            await _store.SyncAsync();

            var teachingEvents = DbContext.TeachingEvents.Include(te => te.Building).ToList();
            teachingEvents.Select(te => te.Name).ToList().ForEach(name => name.Should().Contain("Updated"));
            teachingEvents.Where(te => te.Building != null).Select(te => te.Building.AddressLine1).ToList()
                .ForEach(line1 => line1.Should().Contain("Updated"));
            DbContext.TeachingEvents.Count().Should().Be(8);
            DbContext.TeachingEventBuildings.Count().Should().Be(5);
        }

        [Fact]
        public async Task SyncAsync_DeletesOrphanedTeachingEvents()
        {
            await SeedMockTeachingEventsAndBuildingsAsync();
            var teachingEvent = MockTeachingEvents().ToList().GetRange(0, 1);
            _mockCrm.Setup(m => m.GetTeachingEvents(It.Is<DateTime>(d => CheckGetTeachingEventsAfterDate(d))))
                .Returns(teachingEvent);

            await _store.SyncAsync();

            DbContext.TeachingEvents.Should().BeEquivalentTo(teachingEvent);

            DbContext.TeachingEventBuildings.Should().Contain(teachingEvent.Single().Building);
        }

        [Fact]
        public async Task SyncAsync_InsertsNewTeachingEventBuildings()
        {
            var mockTeachingEventBuildings = MockTeachingEventBuildings();
            _mockCrm.Setup(m => m.GetTeachingEventBuildings()).Returns(mockTeachingEventBuildings);

            await _store.SyncAsync();

            var ids = DbContext.TeachingEventBuildings.Select(te => te.Id);
            ids.Should().BeEquivalentTo(mockTeachingEventBuildings.Select(te => te.Id));
            DbContext.TeachingEventBuildings.Count().Should().Be(5);
        }

        [Fact]
        public async Task SyncAsync_UpdatesExistingTeachingEventBuildings()
        {
            var updatedTeachingEventBuildings = (await SeedMockTeachingEventBuildingsAsync()).ToList();
            updatedTeachingEventBuildings.ForEach(building =>
            {
                building.AddressLine1 += "Updated";
            });
            _mockCrm.Setup(m => m.GetTeachingEventBuildings()).Returns(updatedTeachingEventBuildings);

            await _store.SyncAsync();

            var teachingEventBuildings = DbContext.TeachingEventBuildings;
            teachingEventBuildings.Select(building => building.AddressLine1).ToList().ForEach(name => name.Should().Contain("Updated"));
            DbContext.TeachingEventBuildings.Count().Should().Be(5);
        }

        [Fact]
        public async Task SyncAsync_DeletesOrphanedTeachingEventBuildings()
        {
            var buildings = await SeedMockTeachingEventBuildingsAsync();
            var building = buildings.ToList().GetRange(0, 1);

            _mockCrm.Setup(m => m.GetTeachingEventBuildings()).Returns(building);

            await _store.SyncAsync();

            DbContext.TeachingEventBuildings.Should().BeEquivalentTo(building);
        }

        [Fact]
        public async Task SyncAsync_PopulatesTeachingEventBuildingCoordinates()
        {
            await SeedMockTeachingEventBuildingsAsync();
            SeedMockLocations();

            Store.ClearFailedPostcodeLookupCache();

            _mockCrm.Setup(m => m.GetTeachingEvents(It.Is<DateTime>(d => CheckGetTeachingEventsAfterDate(d)))).Returns(MockTeachingEvents);

            await _store.SyncAsync();

            var teachingEvent = DbContext.TeachingEvents.Include(te => te.Building).First(te => te.Id == FindEventGuid);
            teachingEvent.Building.Coordinate.Should().Be(new Point(new Coordinate(-3.3587, 56.02748)));
        }

        [Fact]
        public async Task SyncAsync_FallbackToGeocodeClient_PopulatesTeachingEventBuildingCoordinatesAndCachesLocation()
        {
            await SeedMockTeachingEventBuildingsAsync();
            SeedMockLocations();

            Store.ClearFailedPostcodeLookupCache();

            _mockCrm.Setup(m => m.GetTeachingEvents(It.Is<DateTime>(d => CheckGetTeachingEventsAfterDate(d)))).Returns(MockTeachingEvents);
            var postcode = "TE7 9IN";
            var coordinate = new Point(1, 2);
            var sanitizedPostcode = Location.SanitizePostcode(postcode);
            _mockGeocodeClient.Setup(m => m.GeocodePostcodeAsync(sanitizedPostcode))
                .ReturnsAsync(coordinate);
            DbContext.Locations.FirstOrDefault(l => l.Postcode == sanitizedPostcode).Should().BeNull();

            await _store.SyncAsync();

            var teachingEvent = DbContext.TeachingEvents.Include(te => te.Building)
                .First(te => te.Building.AddressPostcode == postcode);
            teachingEvent.Building.Coordinate.Should().Be(coordinate);
            DbContext.Locations.FirstOrDefault(l => (l.Postcode == sanitizedPostcode) &&
                                                    (l.Source == Location.SourceType.Google)).Should().NotBeNull();
        }

        [Fact]
        public async Task SyncAsync_InsertsNewPrivacyPolicies()
        {
            var mockPolicies = MockPrivacyPolicies().ToList();
            _mockCrm.Setup(m => m.GetPrivacyPolicies()).Returns(mockPolicies);

            await _store.SyncAsync();

            var ids = DbContext.PrivacyPolicies.Select(p => p.Id);
            ids.Should().BeEquivalentTo(mockPolicies.Select(p => p.Id));
            DbContext.PrivacyPolicies.Count().Should().Be(3);
        }

        [Fact]
        public async Task SyncAsync_UpdatesExistingPrivacyPolicies()
        {
            var updatedPolicies = (await SeedMockPrivacyPoliciesAsync()).ToList();
            updatedPolicies.ForEach(te => te.Text += "Updated");
            _mockCrm.Setup(m => m.GetPrivacyPolicies()).Returns(updatedPolicies);

            await _store.SyncAsync();

            var policies = DbContext.PrivacyPolicies.ToList();
            policies.Select(te => te.Text).ToList().ForEach(name => name.Should().Contain("Updated"));
            DbContext.PrivacyPolicies.Count().Should().Be(3);
        }

        [Fact]
        public async Task SyncAsync_DeletesOrphanedPrivacyPolicies()
        {
            var policies = (await SeedMockPrivacyPoliciesAsync()).ToList();
            _mockCrm.Setup(m => m.GetPrivacyPolicies()).Returns(policies.GetRange(0, 2));

            await _store.SyncAsync();

            var remainingPolicies = DbContext.PrivacyPolicies.ToArray();
            remainingPolicies.Should().BeEquivalentTo(policies.GetRange(0, 2));
        }

        [Fact]
        public async Task SyncAsync_InsertsNewLookupItems()
        {
            var mockCountries = MockCountries();
            _mockCrm.Setup(m => m.GetCountries()).Returns(mockCountries);

            await _store.SyncAsync();

            var ids = DbContext.Countries.Select(e => e.Id);
            ids.Should().BeEquivalentTo(mockCountries.Select(e => e.Id));
            DbContext.Countries.Count().Should().Be(3);
        }

        [Fact]
        public async Task SyncAsync_UpdatesExistingLookupItems()
        {
            var updatedCountries = (await SeedMockCountriesAsync()).ToList();
            updatedCountries.ForEach(c => c.Value += "Updated");
            _mockCrm.Setup(m => m.GetCountries()).Returns((IEnumerable<Country>)updatedCountries);

            await _store.SyncAsync();

            var countries = DbContext.Countries.ToList();
            countries.Select(c => c.Value).ToList().ForEach(value => value.Should().Contain("Updated"));
            DbContext.Countries.Count().Should().Be(3);
        }

        [Fact]
        public async Task SyncAsync_DeletesOrphanedLookupItems()
        {
            var countries = (await SeedMockCountriesAsync()).ToList();
            _mockCrm.Setup(m => m.GetCountries()).Returns((IEnumerable<Country>)countries.GetRange(0, 2));

            await _store.SyncAsync();

            var remainingCountries = DbContext.Countries.ToArray();
            remainingCountries.Should().BeEquivalentTo(countries.GetRange(0, 2));
        }

        [Fact]
        public async Task SyncAsync_InsertsNewPickListItems()
        {
            var mockYears = MockInitialTeacherTrainingYears().ToList();
            _mockCrm.Setup(m => m.GetPickListItems("contact", "dfe_ittyear")).Returns(mockYears);

            await _store.SyncAsync();

            var ids = DbContext.PickListItems.Select(e => e.Id);
            ids.Should().BeEquivalentTo(mockYears.Select(e => e.Id));
            DbContext.PickListItems.Count().Should().Be(3);
        }

        [Fact]
        public async Task SyncAsync_UpdatesExistingPickListItems()
        {
            var updatedYears = (await SeedMockInitialTeacherTrainingYearsAsync()).ToList();
            updatedYears.ForEach(c => c.Value += "Updated");
            _mockCrm.Setup(m => m.GetPickListItems("contact", "dfe_ittyear")).Returns(updatedYears);

            await _store.SyncAsync();

            var countries = DbContext.PickListItems.ToList();
            countries.Select(c => c.Value).ToList().ForEach(value => value.Should().Contain("Updated"));
            DbContext.PickListItems.Count().Should().Be(3);
        }

        [Fact]
        public async Task SyncAsync_InsertsApplyPickListItems()
        {
            _mockCrm.Setup(m => m.GetPickListItems("contact", "dfe_candidateapplystatus")).Returns(Array.Empty<PickListItem>()).Verifiable();
            _mockCrm.Setup(m => m.GetPickListItems("contact", "dfe_candidateapplyphase")).Returns(Array.Empty<PickListItem>()).Verifiable();
            _mockCrm.Setup(m => m.GetPickListItems("dfe_applyapplicationform", "dfe_applyphase")).Returns(Array.Empty<PickListItem>()).Verifiable();
            _mockCrm.Setup(m => m.GetPickListItems("dfe_applyapplicationform", "dfe_applystatus")).Returns(Array.Empty<PickListItem>()).Verifiable();
            _mockCrm.Setup(m => m.GetPickListItems("dfe_applyapplicationchoice", "dfe_applicationchoicestatus")).Returns(Array.Empty<PickListItem>()).Verifiable();
            _mockCrm.Setup(m => m.GetPickListItems("dfe_applyreference", "dfe_referencefeedbackstatus")).Returns(Array.Empty<PickListItem>()).Verifiable();

            await _store.SyncAsync();

            _mockCrm.Verify();
        }

        [Fact]
        public async Task SyncAsync_InsertsRegionIdPickListItem()
        {
            _mockCrm.Setup(m => m.GetPickListItems("msevtmgt_event", "dfe_eventregion")).Returns(Array.Empty<PickListItem>()).Verifiable();

            await _store.SyncAsync();

            _mockCrm.Verify();
        }
      
        [Fact]
        public async Task SyncAsync_DeletesOrphanedPickListItems()
        {
            var years = (await SeedMockInitialTeacherTrainingYearsAsync()).ToList();
            _mockCrm.Setup(m => m.GetPickListItems("contact", "dfe_ittyear")).Returns(years.GetRange(0, 2));

            await _store.SyncAsync();

            var remainingCountries = DbContext.PickListItems
                .Where(p => p.EntityName == "contact" && p.AttributeName == "dfe_ittyear").ToArray();
            remainingCountries.Should().BeEquivalentTo(years.GetRange(0, 2));
        }

        [Fact]
        public async Task GetLookupItems_ReturnsMatchingOrderedByIdAscending()
        {
            await SeedMockCountriesAsync();

            var result = _store.GetCountries();

            result.Select(t => t.Value).Should().BeEquivalentTo(new string[] { "Country 1", "Country 2", "Country 3" });
        }

        [Fact]
        public async Task GetPickListItems_ReturnsMatchingOrderedByIdAscending()
        {
            await SeedMockInitialTeacherTrainingYearsAsync();

            var result = _store.GetPickListItems("contact", "dfe_ittyear");

            result.Select(t => t.Value).Should().BeEquivalentTo(new string[] { "2009", "2010", "2011" });
        }

        [Fact]
        public async Task GetPrivacyPolicies_ReturnsAll()
        {
            await SeedMockPrivacyPoliciesAsync();

            var result = _store.GetPrivacyPolicies();

            result.Select(t => t.Text).Should().BeEquivalentTo(new string[] { "Policy 1", "Policy 2", "Policy 3" });
        }

        [Fact]
        public async Task GetLatestPrivacyPolicy_ReturnsMostRecent()
        {
            await SeedMockPrivacyPoliciesAsync();

            var result = await _store.GetLatestPrivacyPolicyAsync();

            result.Text.Should().Be("Policy 2");
        }

        [Fact]
        public async Task GetPrivacyPolicy_ReturnsMatchingPolicy()
        {
            var policies = await SeedMockPrivacyPoliciesAsync();
            var result = await _store.GetPrivacyPolicyAsync((Guid)policies.First().Id);

            result.Id.Should().Be(policies.First().Id);
        }

        [Fact]
        public async Task SearchTeachingEvents_WithoutFilters_ReturnsAll()
        {
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest() { };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(
                new string[] { "Event 7", "Event 2", "Event 4", "Event 1", "Event 3", "Event 5", "Event 6" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEventsAsync_WithInvalidPostcode_IsCached()
        {
            var request = new TeachingEventSearchRequest() { Postcode = "TE7 1NG", Radius = 100 };
            var sanitizedPostcode = Location.SanitizePostcode(request.Postcode);

            _mockGeocodeClient.Setup(m => m.GeocodePostcodeAsync(sanitizedPostcode)).ReturnsAsync(null as Point);

            await _store.SearchTeachingEventsAsync(request);
            await _store.SearchTeachingEventsAsync(request);

            _mockGeocodeClient.Verify(m => m.GeocodePostcodeAsync(sanitizedPostcode), Times.Once);
        }

        [Fact]
        public async Task SearchTeachingEvents_WithFilters_ReturnsMatching()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest()
            {
                Postcode = "KY6 2NJ",
                Radius = 15,
                TypeIds = new int[] { (int)TeachingEvent.EventType.ApplicationWorkshop },
                StartAfter = DateTime.UtcNow,
                StartBefore = DateTime.UtcNow.AddDays(3),
                Online = true
            };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(
                new string[] { "Event 2" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEvents_WithSingleAccessibilityOptionsFilter_ReturnsMatching()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest()
            {
                AccessibilityOptions = [222750005]
            };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(
                ["Event 6"],
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEvents_WithSingleAccessibilityOptionsFilter_ReturnsMultipleMatching()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest()
            {
                AccessibilityOptions = [222750001]
            };

            var results = await _store.SearchTeachingEventsAsync(request);

            results.ToList().Count.Should().Be(2);
            results.Select(e => e.Name).Should().BeEquivalentTo(
                ["Event 4", "Event 1"],
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEvents_WithMultipleAccessibilityOptionsFilter_ReturnsMultipleMatching()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest()
            {
                AccessibilityOptions = [222750001,222750005,222750006]
            };

            var results = await _store.SearchTeachingEventsAsync(request);

            results.ToList().Count.Should().Be(4);
            results.Select(e => e.Name).Should().BeEquivalentTo(
                ["Event 2", "Event 4", "Event 1", "Event 6"],
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEvents_WithFilters_ReturnsEventNarrowlyInRange()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest()
            {
                Postcode = "KY6 2NJ",
                Radius = 13,
                TypeIds = new int[] { (int)TeachingEvent.EventType.ApplicationWorkshop },
                StartAfter = DateTime.UtcNow,
                StartBefore = DateTime.UtcNow.AddDays(3)
            };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(
                new string[] { "Event 2" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEvents_WithFilters_ExcludesEventNarrowlyOutOfRange()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest()
            {
                Postcode = "KY6 2NJ",
                Radius = 12,
                TypeIds = new int[] { (int)TeachingEvent.EventType.ApplicationWorkshop },
                StartAfter = DateTime.UtcNow,
                StartBefore = DateTime.UtcNow.AddDays(3)
            };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEmpty();
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByRadius_ReturnsMatchingAndOnlineEvents()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest() { Postcode = "KY6 2NJ", Radius = 15 };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 2", "Event 3", "Event 1", "Event 5" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByRadiusWithoutPostcode_ReturnsAll()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest() { Radius = 15 };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(
                new string[] { "Event 7", "Event 2", "Event 4", "Event 1", "Event 3", "Event 5", "Event 6" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByRadiusWithOutwardOnlyPostcode_ReturnsMatchingAndOnlineEvents()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest() { Postcode = "KY6", Radius = 15 };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 2", "Event 3", "Event 1", "Event 5" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByRadiusWithFailedPostcodeGeocoding_ReturnsEmpty()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest() { Postcode = "TE7 1NG", Radius = 15 };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByMultipleTypes_ReturnsMatching()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest()
            {
                TypeIds = new int[]
                {
                    (int)TeachingEvent.EventType.TrainToTeachEvent,
                    (int)TeachingEvent.EventType.ApplicationWorkshop
                }
            };

            var results = await _store.SearchTeachingEventsAsync(request);

            results.Select(e => e.Name).Should().Contain(new string[] { "Event 1", "Event 2" });
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByType_ReturnsMatching()
        {
            SeedMockLocations();
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest() { TypeIds = new int[] { (int)TeachingEvent.EventType.ApplicationWorkshop } };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 2" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByStartAfter_ReturnsMatching()
        {
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest() { StartAfter = DateTime.UtcNow.AddDays(6) };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 3", "Event 5", "Event 6" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByStartBefore_ReturnsMatching()
        {
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest() { StartBefore = DateTime.UtcNow.AddDays(6) };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().BeEquivalentTo(new string[] { "Event 7", "Event 2", "Event 4", "Event 1" },
                options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByDefaultStatusId_ReturnsOpenAndClosedEvents()
        {
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest();

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should()
                .Contain(
                new string[] { "Event 1", "Event 2", "Event 3", "Event 4", "Event 5", "Event 6", "Event 7" });
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByStatusId_ReturnsMatching()
        {
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest()
            {
                StatusIds = new int[] { (int)TeachingEvent.Status.Pending }
            };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should()
                .Contain(
                new string[] { "Event 8" });
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByMultipleStatusIds_ReturnsMatching()
        {
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest()
            {
                StatusIds = new int[] { (int)TeachingEvent.Status.Open, (int)TeachingEvent.Status.Pending }
            };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should()
                .Contain(
                new string[] { "Event 1", "Event 2", "Event 3", "Event 4", "Event 5", "Event 6", "Event 8" });
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByOnline_ReturnsOnlyOnlineEvents()
        {
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest() { Online = true };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().Equal(new string[] { "Event 2", "Event 1", "Event 5" });
        }

        [Fact]
        public async Task SearchTeachingEvents_FilteredByOffline_ReturnsOnlyInPersonEvents()
        {
            await SeedMockTeachingEventsAndBuildingsAsync();
            var request = new TeachingEventSearchRequest() { Online = false };

            var result = await _store.SearchTeachingEventsAsync(request);

            result.Select(e => e.Name).Should().Equal(new string[] { "Event 7", "Event 4", "Event 3", "Event 6" });
        }

        [Fact]
        public async Task GetTeachingEventAsync_WithId_ReturnsMatchingEvent()
        {
            var events = await SeedMockTeachingEventsAndBuildingsAsync();
            var result = await _store.GetTeachingEventAsync((Guid)events.First().Id);

            result.Id.Should().Be(events.First().Id);
            result.Building.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTeachingEventAsync_WithReadableId_ReturnsMatchingEvent()
        {
            var events = await SeedMockTeachingEventsAndBuildingsAsync();
            var result = await _store.GetTeachingEventAsync(events.First().ReadableId);

            result.ReadableId.Should().Be(events.First().ReadableId);
            result.Building.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTeachingEventBuildings_ReturnsAll()
        {
            await SeedMockTeachingEventBuildingsAsync();

            var result = _store.GetTeachingEventBuildings().ToList();

            result.Should().HaveCount(5);
        }

        [Fact]
        public async Task SaveAsync_WithNewModel_Adds()
        {
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid(), Name = "TestEvent" };

            await _store.SaveAsync(teachingEvent);

            DbContext.TeachingEvents
                .FirstOrDefault(e => e.Name == teachingEvent.Name)
                .Should().NotBeNull();
        }

        [Fact]
        public async Task SaveAsync_WithExistingModel_Updates()
        {
            var teachingEvent = new TeachingEvent() { Id = Guid.NewGuid(), Name = "TestEvent" };

            await _store.SaveAsync(teachingEvent);

            teachingEvent.Name += "Updated";

            await _store.SaveAsync(teachingEvent);

            var teachingEventNames = DbContext.TeachingEvents.Select(e => e.Name).ToList();
            teachingEventNames.ForEach(name => name.Should().Contain("Updated"));
        }

        [Fact]
        public async Task SaveAsync_WithModels_Adds()
        {
            var teachingEvent1 = new TeachingEvent() { Id = Guid.NewGuid(), Name = "TestEvent1" };
            var teachingEvent2 = new TeachingEvent() { Id = Guid.NewGuid(), Name = "TestEvent2" };

            await _store.SaveAsync(new TeachingEvent[] { teachingEvent1, teachingEvent2 });

            DbContext.TeachingEvents.FirstOrDefault(e => e.Id == teachingEvent1.Id).Should().NotBeNull();
            DbContext.TeachingEvents.FirstOrDefault(e => e.Id == teachingEvent2.Id).Should().NotBeNull();
        }

        [Fact]
        public async Task SyncMultiItemPickListEntity_WithValidParameters_ReturnsTaskCompletedSucessfully()
        {
            // arrange
            const int DefaultOptionSetValue = 123456;
            const string MethodNameKey = "SyncMultiItemPickListEntity";
            const string EntityNameKey = "msevtmgt_event";
            const string AttributeNameKey = "dfe_accessibility";
            const string DefaultTestEntityName = "TestEntity";
            const string DefaultTestAttributeNameValue = "TestAttribute";

            MethodInfo methodInfo =
                typeof(Store).GetMethod(
                    MethodNameKey,
                    BindingFlags.NonPublic | BindingFlags.Instance);

            _mockCrm.Setup(crmService =>
                crmService.GetMultiplePickListItems(EntityNameKey, AttributeNameKey))
                    .Returns([
                        new(DefaultTestEntityName){
                            FormattedValues = {
                                    [AttributeNameKey] = DefaultTestAttributeNameValue
                            },
                            Attributes = {
                                [AttributeNameKey] =
                                    new List<OptionSetValue>(){new(DefaultOptionSetValue) }
                            }
                        },
                    ])
                    .Verifiable();

            // act
            Task task =
                (Task)methodInfo.Invoke(
                    _store, [EntityNameKey, AttributeNameKey]);

            await task;

            // assert
            Assert.True(task.IsCompletedSuccessfully, "Method did not complete successfully.");

            // verify
            _mockCrm.Verify(crmService =>
                crmService.GetMultiplePickListItems(EntityNameKey, AttributeNameKey), Times.Once);

        }

        // Accessibility options filter expression builder tests.
        //
        [Fact]
        public void BuildAccessibilityFilter_MatchesSingleId()
        {
            // Arrange
            var events = new List<TeachingEvent>
            {
                new TeachingEvent { AccessibilityOptionId = "111,222750001,333" },
                new TeachingEvent { AccessibilityOptionId = "444,555" },
            }
            .AsQueryable();

            var ids = new[] { "222750001" };

            // Act
            var filter = TeachingEventFilterBuilder.BuildAccessibilityFilter(ids);
            var result = events.Where(filter).ToList();

            // Assert
            Assert.Single(result);
            Assert.Contains("222750001", result[0].AccessibilityOptionId);
        }

        [Fact]
        public void BuildAccessibilityFilter_MatchesMultipleIds()
        {
            var events = new List<TeachingEvent>
            {
                new TeachingEvent { AccessibilityOptionId = "222750006" },
                new TeachingEvent { AccessibilityOptionId = "222750001,555" },
                new TeachingEvent { AccessibilityOptionId = "100,101" }
            }
            .AsQueryable();

            var ids = new[] { "222750001", "222750006" };
            var filter = TeachingEventFilterBuilder.BuildAccessibilityFilter(ids);
            var result = events.Where(filter).ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void BuildAccessibilityFilter_ExcludesNonMatches()
        {
            var events = new List<TeachingEvent>
            {
                new TeachingEvent { AccessibilityOptionId = "1,2,3" },
                new TeachingEvent { AccessibilityOptionId = "4,5" }
            }
            .AsQueryable();

            var ids = new[] { "999" };
            var filter = TeachingEventFilterBuilder.BuildAccessibilityFilter(ids);
            var result = events.Where(filter).ToList();

            Assert.Empty(result);
        }

        [Fact]
        public void BuildAccessibilityFilter_IgnoresNulls()
        {
            var events = new List<TeachingEvent>
            {
                new TeachingEvent { AccessibilityOptionId = null },
                new TeachingEvent { AccessibilityOptionId = "222750005" }
            }
            .AsQueryable();

            var ids = new[] { "222750005" };
            var filter = TeachingEventFilterBuilder.BuildAccessibilityFilter(ids);
            var result = events.Where(filter).ToList();

            Assert.Single(result);
        }

        private static bool CheckGetTeachingEventsAfterDate(DateTime date)
        {
            var afterDate = DateTime.UtcNow.Subtract(Store.TeachingEventArchiveSize);

            date.Should().BeCloseTo(afterDate, TimeSpan.FromSeconds(30));

            return true;
        }

        private static List<TeachingEvent> MockTeachingEvents()
        {
            var buildings = MockTeachingEventBuildings();
            var sharedBuildingId = buildings[0].Id;

            var event1 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                ReadableId = "1",
                StatusId = (int)TeachingEvent.Status.Open,
                Name = "Event 1",
                TypeId = (int)TeachingEvent.EventType.TrainToTeachEvent,
                IsOnline = true,
                StartAt = DateTime.UtcNow.AddDays(5),
                BuildingId = sharedBuildingId,
                AccessibilityOptionId = "222750001"
            };

            var event2 = new TeachingEvent()
            {
                Id = FindEventGuid,
                ReadableId = "2",
                StatusId = (int)TeachingEvent.Status.Open,
                IsOnline = true,
                Name = "Event 2",
                StartAt = DateTime.UtcNow.AddDays(1),
                TypeId = (int)TeachingEvent.EventType.ApplicationWorkshop,
                BuildingId = buildings[1].Id,
                AccessibilityOptionId = "222750006"
            };

            var event3 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                ReadableId = "3",
                StatusId = (int)TeachingEvent.Status.Open,
                IsOnline = false,
                Name = "Event 3",
                StartAt = DateTime.UtcNow.AddDays(10),
                TypeId = (int)TeachingEvent.EventType.SchoolOrUniversityEvent,
                BuildingId = buildings[2].Id
            };

            var event4 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                ReadableId = "4",
                StatusId = (int)TeachingEvent.Status.Open,
                IsOnline = false,
                Name = "Event 4",
                StartAt = DateTime.UtcNow.AddDays(3),
                TypeId = (int)TeachingEvent.EventType.SchoolOrUniversityEvent,
                BuildingId = buildings[3].Id,
                AccessibilityOptionId = "222750001,222750002"
            };

            var event5 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                ReadableId = "5",
                StatusId = (int)TeachingEvent.Status.Open,
                Name = "Event 5",
                IsOnline = true,
                TypeId = (int)TeachingEvent.EventType.OnlineEvent,
                StartAt = DateTime.UtcNow.AddDays(15),
            };

            var event6 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                ReadableId = "6",
                StatusId = (int)TeachingEvent.Status.Open,
                Name = "Event 6",
                IsOnline = false,
                StartAt = DateTime.UtcNow.AddDays(60),
                TypeId = (int)TeachingEvent.EventType.SchoolOrUniversityEvent,
                BuildingId = sharedBuildingId,
                AccessibilityOptionId = "222750005"
            };

            var event7 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                ReadableId = "7",
                StatusId = (int)TeachingEvent.Status.Closed,
                Name = "Event 7",
                IsOnline = false,
                StartAt = DateTime.UtcNow.AddYears(-1),
                TypeId = (int)TeachingEvent.EventType.SchoolOrUniversityEvent,
                BuildingId = buildings[4].Id
            };

            var event8 = new TeachingEvent()
            {
                Id = Guid.NewGuid(),
                ReadableId = "8",
                Name = "Event 8",
                IsOnline = false,
                TypeId = (int)TeachingEvent.EventType.QuestionTime,
                StatusId = (int)TeachingEvent.Status.Pending
            };

            return new List<TeachingEvent>() { event1, event2, event3, event4, event5, event6, event7, event8 };
        }

        private static List<TeachingEventBuilding> MockTeachingEventBuildings()
        {
            var building1 = new TeachingEventBuilding()
            {
                Id = new Guid("67ffca5c-5adc-4a63-abb7-632c9ecbf283")
            };

            var building2 = new TeachingEventBuilding()
            {
                Id = new Guid("194c0926-5f15-434d-88ba-f76c376ac865"),
                AddressLine1 = "Line 1",
                AddressPostcode = "KY11 9YU"
            };

            var building3 = new TeachingEventBuilding()
            {
                Id = new Guid("6dc656a8-50cc-4802-a62a-47576ddbc493"),
                AddressLine1 = "Line 1",
                AddressPostcode = "KY6 2NJ",
            };

            var building4 = new TeachingEventBuilding()
            {
                Id = new Guid("adc3e6ce-65a8-4752-abcd-781365982a33"),
                AddressLine1 = "Line 1",
                AddressPostcode = "CA4 8LE"
            };

            var building5 = new TeachingEventBuilding()
            {
                Id = new Guid("deb12260-84fc-43b5-8682-13aa1015f100"),
                AddressPostcode = "TE7 9IN"
            };

            return new List<TeachingEventBuilding> { building1, building2, building3, building4, building5 };
        }

        private async Task<IEnumerable<TeachingEventBuilding>> SeedMockTeachingEventBuildingsAsync()
        {
            var buildings = MockTeachingEventBuildings().ToList();
            _mockCrm.Setup(m => m.GetTeachingEventBuildings()).Returns(buildings);

            await _store.SyncAsync();

            return buildings;
        }

        private async Task<IEnumerable<TeachingEvent>> SeedMockTeachingEventsAndBuildingsAsync()
        {
            await SeedMockTeachingEventBuildingsAsync();

            var teachingEvents = MockTeachingEvents().ToList();
            _mockCrm.Setup(m => m.GetTeachingEvents(It.IsAny<DateTime>())).Returns(teachingEvents);

            await _store.SyncAsync();

            return teachingEvents;
        }

        private static IEnumerable<PrivacyPolicy> MockPrivacyPolicies()
        {
            var policy1 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Policy 1", CreatedAt = DateTime.UtcNow.AddDays(-10) };
            var policy2 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Policy 2", CreatedAt = DateTime.UtcNow };
            var policy3 = new PrivacyPolicy() { Id = Guid.NewGuid(), Text = "Policy 3", CreatedAt = DateTime.UtcNow.AddDays(-5) };

            return new[] { policy1, policy2, policy3 };
        }

        private async Task<IEnumerable<PrivacyPolicy>> SeedMockPrivacyPoliciesAsync()
        {
            var privacyPolicies = MockPrivacyPolicies().ToList();
            _mockCrm.Setup(m => m.GetPrivacyPolicies()).Returns(privacyPolicies);

            await _store.SyncAsync();

            return privacyPolicies;
        }

        private static IEnumerable<Country> MockCountries()
        {
            var country1 = new Country() { Id = new Guid("00000000-0000-0000-0000-000000000000"), Value = "Country 1" };
            var country2 = new Country() { Id = new Guid("00000000-0000-0000-0000-000000000001"), Value = "Country 2" };
            var country3 = new Country() { Id = new Guid("00000000-0000-0000-0000-000000000002"), Value = "Country 3" };

            return new Country[] { country2, country1, country3 };
        }

        private async Task<IEnumerable<Country>> SeedMockCountriesAsync()
        {
            var countries = MockCountries();
            _mockCrm.Setup(m => m.GetCountries()).Returns(countries);

            await _store.SyncAsync();

            return countries;
        }

        private static IEnumerable<PickListItem> MockInitialTeacherTrainingYears()
        {
            var year1 = new PickListItem() { Id = 1, Value = "2009", EntityName = "contact", AttributeName = "dfe_ittyear" };
            var year2 = new PickListItem() { Id = 2, Value = "2010", EntityName = "contact", AttributeName = "dfe_ittyear" };
            var year3 = new PickListItem() { Id = 3, Value = "2011", EntityName = "contact", AttributeName = "dfe_ittyear" };

            return new PickListItem[] { year2, year1, year3 };
        }

        private async Task<IEnumerable<PickListItem>> SeedMockInitialTeacherTrainingYearsAsync()
        {
            var items = MockInitialTeacherTrainingYears().ToList();
            _mockCrm.Setup(m => m.GetPickListItems("contact", "dfe_ittyear")).Returns(items);

            await _store.SyncAsync();

            return items;
        }

        private void SeedMockLocations()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: DbConfiguration.Wgs84Srid);
            var locations = new Location[]
            {
                new GetIntoTeachingApi.Models.Location() {Postcode = "ky119yu", Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.35870, 56.02748))},
                new Location() {Postcode = "ca48le", Coordinate = geometryFactory.CreatePoint(new Coordinate(-2.84000, 54.89014))},
                new Location() {Postcode = "ky62nj", Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.178240, 56.182790))},
                new Location() {Postcode = "kw14yl", Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.10075, 58.64102))},
                new Location() {Postcode = "tr182ab", Coordinate = geometryFactory.CreatePoint(new Coordinate(-5.53987, 50.12279))},
                new Location() {Postcode = "ky6", Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.224217, 56.217468))},
            };

            DbContext.Locations.AddRange(locations);
            DbContext.SaveChanges();
        }
    }
}
