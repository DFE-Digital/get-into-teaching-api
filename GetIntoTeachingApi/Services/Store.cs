using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Utils;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NetTopologySuite.Geometries;
using Location = GetIntoTeachingApi.Models.Location;

namespace GetIntoTeachingApi.Services
{
    public class Store : IStore
    {
        private readonly GetIntoTeachingDbContext _dbContext;
        private readonly IGeocodeClientAdapter _geocodeClient;
        private readonly ICrmService _crm;

        public Store(GetIntoTeachingDbContext dbContext, IGeocodeClientAdapter geocodeClient, ICrmService crm)
        {
            _dbContext = dbContext;
            _geocodeClient = geocodeClient;
            _crm = crm;
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
            await SyncTeachingEvents();
            await SyncPrivacyPolicies();
            await SyncTypeEntities();
            await SyncLookupItems();
            await SyncPickListItems();
        }

        public IQueryable<LookupItem> GetLookupItems(string entityName)
        {
            return _dbContext.LookupItems.Where(t => t.EntityName == entityName).OrderBy(t => t.Id);
        }

        public IQueryable<PickListItem> GetPickListItems(string entityName, string attributeName)
        {
            return _dbContext.PickListItems.Where(t => t.EntityName == entityName && t.AttributeName == attributeName).OrderBy(t => t.Id);
        }

        public IQueryable<TypeEntity> GetTypeEntitites(string entityName, string attributeName = null)
        {
            var query = _dbContext.TypeEntities.Where(t => t.EntityName == entityName);

            if (attributeName != null)
            {
                query = query.Where(t => t.AttributeName == attributeName);
            }

            return query.OrderBy(t => t.Id);
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
            return _dbContext.PrivacyPolicies;
        }

        public async Task<IEnumerable<TeachingEvent>> SearchTeachingEventsAsync(TeachingEventSearchRequest request)
        {
            IQueryable<TeachingEvent> teachingEvents = _dbContext.TeachingEvents
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

            if (request.Radius == null)
            {
                return await teachingEvents.ToListAsync();
            }

            return await FilterTeachingEventsByRadius(teachingEvents, request);
        }

        public async Task<TeachingEvent> GetTeachingEventAsync(Guid id)
        {
            return await _dbContext.TeachingEvents.Include(e => e.Building).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<TeachingEvent> GetTeachingEventAsync(string readableId)
        {
            return await _dbContext.TeachingEvents.Include(e => e.Building).FirstOrDefaultAsync(e => e.ReadableId == readableId);
        }

        public IQueryable<TeachingEvent> GetUpcomingTeachingEvents()
        {
            return _dbContext.TeachingEvents
                .Include(e => e.Building)
                .OrderBy(e => e.StartAt)
                .Where(e => e.StartAt >= DateTime.UtcNow);
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

            var inMemoryTeachingEvents = await teachingEvents.ToListAsync();

            // Project coordinates onto UK coordinate system for final, accurate distance filtering.
            var result = inMemoryTeachingEvents.Where(te => te.Building.Coordinate.ProjectTo(DbConfiguration.UkSrid)
                    .IsWithinDistance(origin.ProjectTo(DbConfiguration.UkSrid), (double)request.RadiusInKm() * 1000));

            // We need to include online events in distance-based searches (unless explicitly filtered out by the request).
            var includeOnlineEvents = request.TypeId == null || request.TypeId == (int)TeachingEvent.EventType.OnlineEvent;
            if (includeOnlineEvents)
            {
                var onlineEventsRequest = request.Clone(te =>
                {
                    te.TypeId = (int)TeachingEvent.EventType.OnlineEvent;
                    te.Radius = null;
                });

                result = result.Concat(await SearchTeachingEventsAsync(onlineEventsRequest));
            }

            return result;
        }

        private async Task SyncTeachingEvents()
        {
            var teachingEvents = _crm.GetTeachingEvents().ToList();
            await PopulateTeachingEventCoordinates(teachingEvents);

            var buildings = teachingEvents.Where(te => te.Building != null)
                .Select(te => te.Building).DistinctBy(b => b.Id);

            await SyncModels(buildings, _dbContext.TeachingEventBuildings);

            // Link events with buildings attached to the context prior to sync.
            foreach (var te in teachingEvents.Where(te => te.Building != null))
            {
                te.Building = await _dbContext.TeachingEventBuildings.FindAsync(te.Building.Id);
            }

            await SyncModels(teachingEvents, _dbContext.TeachingEvents);
        }

        private async Task SyncPrivacyPolicies()
        {
            var policies = _crm.GetPrivacyPolicies().ToList();
            await SyncModels(policies, _dbContext.PrivacyPolicies);
        }

        private async Task SyncTypeEntities()
        {
            await SyncTypes("dfe_country");
            await SyncTypes("dfe_teachingsubjectlist");
            await SyncTypes("contact", "dfe_ittyear");
            await SyncTypes("contact", "dfe_preferrededucationphase01");
            await SyncTypes("contact", "dfe_channelcreation");
            await SyncTypes("contact", "dfe_websitehasgcseenglish");
            await SyncTypes("contact", "dfe_websiteplanningretakeenglishgcse");
            await SyncTypes("contact", "dfe_websitewhereinconsiderationjourney");
            await SyncTypes("contact", "dfe_typeofcandidate");
            await SyncTypes("contact", "dfe_candidatestatus");
            await SyncTypes("contact", "dfe_iscandidateeligibleforadviser");
            await SyncTypes("contact", "dfe_isadvisorrequiredos");
            await SyncTypes("contact", "dfe_gitismlservicesubscriptionchannel");
            await SyncTypes("contact", "dfe_gitiseventsservicesubscriptionchannel");
            await SyncTypes("dfe_candidatequalification", "dfe_degreestatus");
            await SyncTypes("dfe_candidatequalification", "dfe_ukdegreegrade");
            await SyncTypes("dfe_candidatequalification", "dfe_type");
            await SyncTypes("dfe_candidatepastteachingposition", "dfe_educationphase");
            await SyncTypes("msevtmgt_event", "dfe_event_type");
            await SyncTypes("msevtmgt_event", "dfe_eventstatus");
            await SyncTypes("msevtmgt_eventregistration", "dfe_channelcreation");
            await SyncTypes("phonecall", "dfe_channelcreation");
            await SyncTypes("dfe_servicesubscription", "dfe_servicesubscriptiontype");
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

        private async Task SyncTypes(string entityName, string attributeName = null)
        {
            var types = _crm.GetTypeEntities(entityName, attributeName);

            if (!types.Any())
            {
                return;
            }

            var key = types.Select(t => new { t.EntityName, t.AttributeName }).First();
            var typeIds = types.Select(t => t.Id);
            var existingIds = _dbContext.TypeEntities
                .Where(t => t.EntityName == key.EntityName && t.AttributeName == key.AttributeName)
                .Select(t => t.Id);

            _dbContext.RemoveRange(_dbContext.TypeEntities.Where(t => t.EntityName == key.EntityName
                && t.AttributeName == key.AttributeName && !typeIds.Contains(t.Id)));
            _dbContext.UpdateRange(types.Where(t => existingIds.Contains(t.Id)));
            await _dbContext.AddRangeAsync(types.Where(t => !existingIds.Contains(t.Id)));
            await _dbContext.SaveChangesAsync();
        }

        private async Task PopulateTeachingEventCoordinates(IEnumerable<TeachingEvent> teachingEvents)
        {
            foreach (var teachingEvent in teachingEvents.Where(te => te.Building?.AddressPostcode != null))
            {
                var coordinate = await CoordinateForPostcode(teachingEvent.Building.AddressPostcode);
                teachingEvent.Building.Coordinate = coordinate;
            }
        }

        private async Task<Point> CoordinateForPostcode(string postcode)
        {
            var sanitizedPostcode = Location.SanitizePostcode(postcode);

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
