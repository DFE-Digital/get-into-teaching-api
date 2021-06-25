using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NetTopologySuite.Geometries;
using Location = GetIntoTeachingApi.Models.Location;

namespace GetIntoTeachingApi.Services
{
    public class Store : IStore
    {
        public static readonly TimeSpan TeachingEventArchiveSize = TimeSpan.FromDays(31 * 4);
        public static readonly HashSet<string> FailedPostcodeLookupCache = new HashSet<string>();
        private readonly GetIntoTeachingDbContext _dbContext;
        private readonly IGeocodeClientAdapter _geocodeClient;
        private readonly ICrmService _crm;
        private readonly IDateTimeProvider _dateTime;

        public Store(
            GetIntoTeachingDbContext dbContext,
            IGeocodeClientAdapter geocodeClient,
            ICrmService crm,
            IDateTimeProvider dateTime)
        {
            _dbContext = dbContext;
            _geocodeClient = geocodeClient;
            _crm = crm;
            _dateTime = dateTime;
        }

        public async Task<string> CheckStatusAsync()
        {
            try
            {
                await _dbContext.Database.OpenConnectionAsync();
                await _dbContext.Database.CloseConnectionAsync();
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return HealthCheckResponse.StatusOk;
        }

        public async Task SyncAsync()
        {
            await SyncTeachingEventBuildings();
            await SyncTeachingEvents();
            await SyncPrivacyPolicies();
            await SyncLookupItems();
            await SyncPickListItems();
        }

        public IQueryable<LookupItem> GetLookupItems(string entityName)
        {
            return _dbContext.LookupItems.AsNoTracking().Where(t => t.EntityName == entityName).OrderBy(t => t.Id);
        }

        public IQueryable<PickListItem> GetPickListItems(string entityName, string attributeName)
        {
            return _dbContext.PickListItems.AsNoTracking().Where(t => t.EntityName == entityName && t.AttributeName == attributeName).OrderBy(t => t.Id);
        }

        public async Task<PrivacyPolicy> GetLatestPrivacyPolicyAsync()
        {
            return await GetPrivacyPolicies().OrderByDescending(p => p.CreatedAt).FirstOrDefaultAsync();
        }

        public async Task<PrivacyPolicy> GetPrivacyPolicyAsync(Guid id)
        {
            return await GetPrivacyPolicies().FirstOrDefaultAsync(teachingEvent => teachingEvent.Id == id);
        }

        public IQueryable<PrivacyPolicy> GetPrivacyPolicies()
        {
            return _dbContext.PrivacyPolicies.AsNoTracking();
        }

        public async Task<IEnumerable<TeachingEvent>> SearchTeachingEventsAsync(TeachingEventSearchRequest request)
        {
            IQueryable<TeachingEvent> teachingEvents = _dbContext.TeachingEvents
                .AsNoTracking()
                .Include(te => te.Building)
                .OrderBy(te => te.StartAt);

            if (request.TypeId != null)
            {
                teachingEvents = teachingEvents.Where(te => te.TypeId == request.TypeId);
            }

            if (request.StartAfter != null)
            {
                teachingEvents = teachingEvents.Where(te => request.StartAfter < te.StartAt);
            }

            if (request.StartBefore != null)
            {
                teachingEvents = teachingEvents.Where(te => request.StartBefore > te.StartAt);
            }

            if (request.StatusIds != null)
            {
                teachingEvents = teachingEvents.Where(te =>
                    request.StatusIds.Contains(te.StatusId));
            }

            if (request.Radius == null)
            {
                return await teachingEvents.ToListAsync();
            }

            return await FilterTeachingEventsByRadius(teachingEvents, request);
        }

        public async Task<TeachingEvent> GetTeachingEventAsync(Guid id)
        {
            return await _dbContext.TeachingEvents.AsNoTracking().Include(e => e.Building).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<TeachingEvent> GetTeachingEventAsync(string readableId)
        {
            return await _dbContext.TeachingEvents.AsNoTracking().Include(e => e.Building).FirstOrDefaultAsync(e => e.ReadableId == readableId);
        }

        public IQueryable<TeachingEventBuilding> GetTeachingEventBuildings()
        {
            return _dbContext.TeachingEventBuildings.AsNoTracking();
        }

        public async Task SaveAsync<T>(IEnumerable<T> models)
            where T : BaseModel
        {
            var existingIds = _dbContext.Set<T>().Select(m => m.Id);

            _dbContext.UpdateRange(models.Where(m => existingIds.Contains(m.Id)));
            await _dbContext.AddRangeAsync(models.Where(m => !existingIds.Contains(m.Id)));
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveAsync<T>(T model)
            where T : BaseModel
        {
            await SaveAsync(new T[] { model });
        }

        private async Task<IEnumerable<TeachingEvent>> FilterTeachingEventsByRadius(
            IQueryable<TeachingEvent> teachingEvents, TeachingEventSearchRequest request)
        {
            var origin = await CoordinateForPostcode(request.Postcode);

            // If we can't locate them, return no results.
            if (origin == null)
            {
                return new List<TeachingEvent>();
            }

            // Exclude events we don't have a location for.
            teachingEvents = teachingEvents.Where(te => te.Building != null && te.Building.Coordinate != null);

            // Filter events by distance in database
            var result = teachingEvents.Where(teachingEvent => teachingEvent.Building.Coordinate.Distance(origin)
                < request.RadiusInKm() * 1000).AsEnumerable();

            // We need to include the various 'online' event types in distance based search results.
            result = result.Concat(await OnlineEventsMatchingRequest(request));

            return result;
        }

        private async Task<IEnumerable<TeachingEvent>> OnlineEventsMatchingRequest(TeachingEventSearchRequest originalRequest)
        {
            var request = originalRequest.Clone(te => { te.Radius = null; });
            var result = await SearchTeachingEventsAsync(request);

            return result.Where(te => te.IsOnline && !te.IsVirtual);
        }

        private async Task SyncTeachingEventBuildings()
        {
            var buildings = _crm.GetTeachingEventBuildings();
            await PopulateTeachingEventCoordinates(buildings);

            await SyncModels(buildings, _dbContext.TeachingEventBuildings);
        }

        private async Task SyncTeachingEvents()
        {
            var afterDate = _dateTime.UtcNow.Subtract(TeachingEventArchiveSize);
            var teachingEvents = _crm.GetTeachingEvents(afterDate).ToList();
            var teachingEventBuildings = _dbContext.TeachingEventBuildings.ToList();

            foreach (var te in teachingEvents.Where(te => te.BuildingId != null))
            {
                te.Building = teachingEventBuildings.FirstOrDefault(b => b.Id == te.BuildingId);
            }

            await SyncModels(teachingEvents, _dbContext.TeachingEvents);
        }

        private async Task SyncPrivacyPolicies()
        {
            var policies = _crm.GetPrivacyPolicies().ToList();
            await SyncModels(policies, _dbContext.PrivacyPolicies);
        }

        private async Task SyncLookupItems()
        {
            await SyncLookupItem("dfe_country");
            await SyncLookupItem("dfe_teachingsubjectlist");
        }

        private async Task SyncPickListItems()
        {
            await SyncPickListItem("contact", "dfe_ittyear");
            await SyncPickListItem("contact", "dfe_preferrededucationphase01");
            await SyncPickListItem("contact", "dfe_channelcreation");
            await SyncPickListItem("contact", "dfe_websitehasgcseenglish");
            await SyncPickListItem("contact", "dfe_websiteplanningretakeenglishgcse");
            await SyncPickListItem("contact", "dfe_websitewhereinconsiderationjourney");
            await SyncPickListItem("contact", "dfe_typeofcandidate");
            await SyncPickListItem("contact", "dfe_candidatestatus");
            await SyncPickListItem("contact", "dfe_iscandidateeligibleforadviser");
            await SyncPickListItem("contact", "dfe_isadvisorrequiredos");
            await SyncPickListItem("contact", "dfe_gitismlservicesubscriptionchannel");
            await SyncPickListItem("contact", "dfe_gitiseventsservicesubscriptionchannel");
            await SyncPickListItem("dfe_candidatequalification", "dfe_degreestatus");
            await SyncPickListItem("dfe_candidatequalification", "dfe_ukdegreegrade");
            await SyncPickListItem("dfe_candidatequalification", "dfe_type");
            await SyncPickListItem("dfe_candidatepastteachingposition", "dfe_educationphase");
            await SyncPickListItem("msevtmgt_event", "dfe_event_type");
            await SyncPickListItem("msevtmgt_event", "dfe_eventstatus");
            await SyncPickListItem("msevtmgt_eventregistration", "dfe_channelcreation");
            await SyncPickListItem("phonecall", "dfe_channelcreation");
            await SyncPickListItem("dfe_servicesubscription", "dfe_servicesubscriptiontype");
        }

        private async Task SyncModels<T>(IEnumerable<T> models, IQueryable<T> dbSet)
            where T : BaseModel
        {
            var existingIds = dbSet.Select(m => m.Id);
            var modelIds = models.Select(m => m.Id);

            _dbContext.RemoveRange(dbSet.Where(m => !modelIds.Contains(m.Id)));
            _dbContext.UpdateRange(models.Where(m => existingIds.Contains(m.Id)));
            await _dbContext.AddRangeAsync(models.Where(m => !existingIds.Contains(m.Id)));
            await _dbContext.SaveChangesAsync();
        }

        private async Task SyncLookupItem(string entityName)
        {
            var items = _crm.GetLookupItems(entityName);
            var ids = items.Select(t => t.Id);
            var existingIds = _dbContext.LookupItems
                .Where(t => t.EntityName == entityName)
                .Select(t => t.Id);

            _dbContext.RemoveRange(_dbContext.LookupItems.Where(t => t.EntityName == entityName && !ids.Contains(t.Id)));
            _dbContext.UpdateRange(items.Where(t => existingIds.Contains(t.Id)));
            await _dbContext.AddRangeAsync(items.Where(t => !existingIds.Contains(t.Id)));
            await _dbContext.SaveChangesAsync();
        }

        private async Task SyncPickListItem(string entityName, string attributeName)
        {
            var items = _crm.GetPickListItems(entityName, attributeName);
            var ids = items.Select(t => t.Id);
            var existingIds = _dbContext.PickListItems
                .Where(t => t.EntityName == entityName && t.AttributeName == attributeName)
                .Select(t => t.Id);

            _dbContext.RemoveRange(_dbContext.PickListItems.Where(t => t.EntityName == entityName &&
                t.AttributeName == attributeName && !ids.Contains(t.Id)));
            _dbContext.UpdateRange(items.Where(t => existingIds.Contains(t.Id)));
            await _dbContext.AddRangeAsync(items.Where(t => !existingIds.Contains(t.Id)));
            await _dbContext.SaveChangesAsync();
        }

        private async Task PopulateTeachingEventCoordinates(IEnumerable<TeachingEventBuilding> buildings)
        {
            foreach (var building in buildings.Where(building => building.AddressPostcode != null))
            {
                building.Coordinate = await CoordinateForPostcode(building.AddressPostcode);
            }
        }

        private async Task<Point> CoordinateForPostcode(string postcode)
        {
            var sanitizedPostcode = Location.SanitizePostcode(postcode);

            // Avoid the expense (Google API call) to re-check known lookup failures.
            if (FailedPostcodeLookupCache.Contains(postcode))
            {
                return null;
            }

            var coordinate = await GeocodePostcodeWithLocalLookup(sanitizedPostcode);

            if (coordinate != null)
            {
                return coordinate;
            }

            coordinate = await _geocodeClient.GeocodePostcodeAsync(sanitizedPostcode);

            if (coordinate != null)
            {
                await CacheLocation(postcode, coordinate);
            }
            else
            {
                FailedPostcodeLookupCache.Add(postcode);
            }

            return coordinate;
        }

        private async Task CacheLocation(string postcode, Point coordinate)
        {
            var location = new Location(postcode, coordinate, Location.SourceType.Google);
            await _dbContext.Locations.AddAsync(location);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<Point> GeocodePostcodeWithLocalLookup(string sanitizedPostcode)
        {
            return await _dbContext.Locations.Where(l => l.Postcode == sanitizedPostcode)
                            .Select(l => l.Coordinate).FirstOrDefaultAsync();
        }
    }
}
