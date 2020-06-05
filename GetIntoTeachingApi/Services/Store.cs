using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Sync(ICrmService crm)
        {
            SyncTeachingEvents(crm);
            SyncPrivacyPolicies(crm);
            SyncTypeEntities(crm);
        }

        public IEnumerable<TypeEntity> GetLookupItems(string entityName)
        {
            return _dbContext.TypeEntities.Where(t => t.EntityName == entityName);
        }

        public IEnumerable<TypeEntity> GetPickListItems(string entityName, string attributeName)
        {
            return _dbContext.TypeEntities.Where(t => t.EntityName == entityName && t.AttributeName == attributeName);
        }

        public PrivacyPolicy GetLatestPrivacyPolicy()
        {
            return GetPrivacyPolicies().OrderByDescending(p => p.CreatedAt).FirstOrDefault();
        }

        public IEnumerable<PrivacyPolicy> GetPrivacyPolicies()
        {
            return _dbContext.PrivacyPolicies;
        }

        public IEnumerable<TeachingEvent> GetUpcomingTeachingEvents(int limit)
        {
            return _dbContext.TeachingEvents
                .Where((teachingEvent) => teachingEvent.StartAt > DateTime.Now)
                .OrderBy(teachingEvent => teachingEvent.StartAt)
                .Take(limit);
        }

        public IEnumerable<TeachingEvent> SearchTeachingEvents(TeachingEventSearchRequest request)
        {
            IEnumerable<TeachingEvent> teachingEvents = _dbContext.TeachingEvents.Include(te => te.Building);

            if (request.TypeId != null)
                teachingEvents = teachingEvents.Where(te => te.TypeId == request.TypeId);

            if (request.StartAfter != null)
                teachingEvents = teachingEvents.Where(te => request.StartAfter < te.StartAt);

            if (request.StartBefore != null)
                teachingEvents = teachingEvents.Where(te => request.StartBefore > te.StartAt);

            if (request.Radius != null)
            {
                var origin = CoordinateForPostcode(request.Postcode);

                teachingEvents = teachingEvents.Where(te => te.Building != null && te.Building.Coordinate != null);

                // Approximate distance filtering in the database, with a suitable error margin (treats distance as an arc degree).
                teachingEvents = teachingEvents.Where(te => EarthCircumferenceInKm * 
                    te.Building.Coordinate.Distance(origin) / 360 < request.RadiusInKm + ErrorMarginInKm);

                // Project coordinates in-memory for additional, accurate distance filtering.
                teachingEvents = teachingEvents.ToList().Where(te => 
                    te.Building.Coordinate.ProjectTo(DbConfiguration.UkSrid)
                        .IsWithinDistance(origin.ProjectTo(DbConfiguration.UkSrid), 
                            (double) request.RadiusInKm * 1000));
            }

            return teachingEvents.OrderBy(te => te.StartAt);
        }

        public TeachingEvent GetTeachingEvent(Guid id)
        {
            return _dbContext.TeachingEvents.FirstOrDefault(teachingEvent => teachingEvent.Id == id);
        }

        public bool IsValidPostcode(string postcode)
        {
            if (string.IsNullOrEmpty(postcode))
                return false;

            return _dbContext.Locations.Any(l => l.Postcode == Location.SanitizePostcode(postcode));
        }

        private void SyncTeachingEvents(ICrmService crm)
        {
            var teachingEvents = crm.GetTeachingEvents().ToList();
            PopulateTeachingEventCoordinates(teachingEvents);

            var buildings = teachingEvents.Where(te => te.Building != null)
                .Select(te => te.Building).DistinctBy(b => b.Id);

            UpsertModels(buildings, _dbContext.TeachingEventBuildings);

            // Link events with buildings attached to the context prior to upsert.
            teachingEvents.Where(te => te.Building != null).ToList()
                .ForEach(te => te.Building = _dbContext.TeachingEventBuildings.Find(te.Building.Id));

            UpsertModels(teachingEvents, _dbContext.TeachingEvents);

            _dbContext.SaveChanges();
        }

        private void SyncPrivacyPolicies(ICrmService crm)
        {
            var policies = crm.GetPrivacyPolicies().ToList();
            UpsertModels(policies, _dbContext.PrivacyPolicies);
            _dbContext.SaveChanges();
        }

        private void SyncTypeEntities(ICrmService crm)
        {
            UpsertTypes(crm.GetLookupItems("dfe_country"));
            UpsertTypes(crm.GetLookupItems("dfe_teachingsubjectlist"));
            UpsertTypes(crm.GetPickListItems("contact", "dfe_ittyear"));
            UpsertTypes(crm.GetPickListItems("contact", "dfe_preferrededucationphase01"));
            UpsertTypes(crm.GetPickListItems("contact", "dfe_isinuk"));
            UpsertTypes(crm.GetPickListItems("dfe_qualification", "dfe_degreestatus"));
            UpsertTypes(crm.GetPickListItems("dfe_qualification", "dfe_category"));
            UpsertTypes(crm.GetPickListItems("dfe_qualification", "dfe_type"));
            UpsertTypes(crm.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase"));
            UpsertTypes(crm.GetPickListItems("msevtmgt_event", "dfe_event_type"));
            _dbContext.SaveChanges();
        }

        private void UpsertModels<T>(IEnumerable<T> models, IQueryable<T> dbSet) where T : BaseModel
        {
            var existingIds = dbSet.Select(m => m.Id);
            _dbContext.UpdateRange(models.Where(m => existingIds.Contains(m.Id)));
            _dbContext.AddRange(models.Where(m => !existingIds.Contains(m.Id)));
        }

        private void UpsertTypes(IEnumerable<TypeEntity> types)
        {
            if (!types.Any()) return;

            var key = types.Select(te => new { te.EntityName, te.AttributeName }).First();
            var existingIds = _dbContext.TypeEntities
                .Where(m => m.EntityName == key.EntityName && m.AttributeName == key.AttributeName)
                .Select(m => m.Id);
            _dbContext.UpdateRange(types.Where(m => existingIds.Contains(m.Id)));
            _dbContext.AddRange(types.Where(m => !existingIds.Contains(m.Id)));
            _dbContext.SaveChanges();
        }

        private void PopulateTeachingEventCoordinates(IEnumerable<TeachingEvent> teachingEvents)
        {
            foreach (var teachingEvent in teachingEvents.Where(te => te.Building?.AddressPostcode != null))
            {
                teachingEvent.Building.Coordinate = CoordinateForPostcode(teachingEvent.Building.AddressPostcode);
            }
        }

        private Point CoordinateForPostcode(string postcode)
        {
            var sanitizedPostcode = Location.SanitizePostcode(postcode);
            var location = _dbContext.Locations.FirstOrDefault(l => l.Postcode == sanitizedPostcode);

            return location?.Coordinate;
        }
    }
}
