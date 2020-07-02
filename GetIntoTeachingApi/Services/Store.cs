using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private const double EarthCircumferenceInKm = 40075.017;

        // We get a 16km error when approximating the distance between postcodes for
        // John O'Groats and Lands End.
        private const double ErrorMarginInKm = 25;
        private readonly GetIntoTeachingDbContext _dbContext;

        public Store(GetIntoTeachingDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public async Task SyncAsync(ICrmService crm)
        {
            await SyncTeachingEvents(crm);
            await SyncPrivacyPolicies(crm);
            await SyncTypeEntities(crm);
        }

        public IQueryable<TypeEntity> GetLookupItems(string entityName)
        {
            return _dbContext.TypeEntities.Where(t => t.EntityName == entityName);
        }

        public IQueryable<TypeEntity> GetPickListItems(string entityName, string attributeName)
        {
            return _dbContext.TypeEntities.Where(t => t.EntityName == entityName && t.AttributeName == attributeName);
        }

        public async Task<PrivacyPolicy> GetLatestPrivacyPolicyAsync()
        {
            return await GetPrivacyPolicies().OrderByDescending(p => p.CreatedAt).FirstOrDefaultAsync();
        }

        public IQueryable<PrivacyPolicy> GetPrivacyPolicies()
        {
            return _dbContext.PrivacyPolicies;
        }

        public IQueryable<TeachingEvent> GetUpcomingTeachingEvents(int limit)
        {
            return _dbContext.TeachingEvents
                .Where((teachingEvent) => teachingEvent.StartAt > DateTime.Now)
                .OrderBy(teachingEvent => teachingEvent.StartAt)
                .Take(limit);
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
            return await _dbContext.TeachingEvents.FirstOrDefaultAsync(teachingEvent => teachingEvent.Id == id);
        }

        public bool IsValidPostcode(string postcode)
        {
            if (string.IsNullOrEmpty(postcode))
            {
                return false;
            }

            return _dbContext.Locations.Any(l => l.Postcode == Location.SanitizePostcode(postcode));
        }

        private async Task<IEnumerable<TeachingEvent>> FilterTeachingEventsByRadius(
            IQueryable<TeachingEvent> teachingEvents, TeachingEventSearchRequest request)
        {
            var origin = await CoordinateForPostcode(request.Postcode);

            // Exclude events we don't have a location for.
            teachingEvents = teachingEvents.Where(te => te.Building != null && te.Building.Coordinate != null);

            // Approximate distance filtering in the database, with a suitable error margin (treats distance as an arc degree).
            teachingEvents = teachingEvents.Where(te => EarthCircumferenceInKm *
                te.Building.Coordinate.Distance(origin) / 360 < request.RadiusInKm + ErrorMarginInKm);

            var inMemoryTeachingEvents = await teachingEvents.ToListAsync();

            // Project coordinates onto UK coordinate system for final, accurate distance filtering.
            return inMemoryTeachingEvents.Where(te => te.Building.Coordinate.ProjectTo(DbConfiguration.UkSrid)
                    .IsWithinDistance(origin.ProjectTo(DbConfiguration.UkSrid), (double)request.RadiusInKm * 1000));
        }

        private async Task SyncTeachingEvents(ICrmService crm)
        {
            var teachingEvents = crm.GetTeachingEvents().ToList();
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

            await _dbContext.SaveChangesAsync();
        }

        private async Task SyncPrivacyPolicies(ICrmService crm)
        {
            var policies = crm.GetPrivacyPolicies().ToList();
            await SyncModels(policies, _dbContext.PrivacyPolicies);
        }

        private async Task SyncTypeEntities(ICrmService crm)
        {
            await SyncTypes(crm.GetLookupItems("dfe_country"));
            await SyncTypes(crm.GetLookupItems("dfe_teachingsubjectlist"));
            await SyncTypes(crm.GetPickListItems("contact", "dfe_ittyear"));
            await SyncTypes(crm.GetPickListItems("contact", "dfe_preferrededucationphase01"));
            await SyncTypes(crm.GetPickListItems("contact", "dfe_channelcreation"));
            await SyncTypes(crm.GetPickListItems("contact", "dfe_hasgcseenglish"));
            await SyncTypes(crm.GetPickListItems("contact", "dfe_websiteplanningretakeenglishgcse"));
            await SyncTypes(crm.GetPickListItems("contact", "dfe_websitedescribeyourself"));
            await SyncTypes(crm.GetPickListItems("contact", "dfe_websitewhereinconsiderationjourney"));
            await SyncTypes(crm.GetPickListItems("contact", "dfe_typeofcandidate"));
            await SyncTypes(crm.GetPickListItems("contact", "dfe_candidatestatus"));
            await SyncTypes(crm.GetPickListItems("contact", "dfe_iscandidateeligibleforadviser"));
            await SyncTypes(crm.GetPickListItems("contact", "dfe_isadvisorrequiredos"));
            await SyncTypes(crm.GetPickListItems("dfe_candidatequalification", "dfe_degreestatus"));
            await SyncTypes(crm.GetPickListItems("dfe_candidatequalification", "dfe_ukdegreegrade"));
            await SyncTypes(crm.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase"));
            await SyncTypes(crm.GetPickListItems("msevtmgt_event", "dfe_event_type"));
            await SyncTypes(crm.GetPickListItems("phonecall", "dfe_channelcreation"));
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

        private async Task SyncTypes(IEnumerable<TypeEntity> types)
        {
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
            return await _dbContext.Locations.Where(l => l.Postcode == sanitizedPostcode)
                .Select(l => l.Coordinate).FirstOrDefaultAsync();
        }
    }
}
