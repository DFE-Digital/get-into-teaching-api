﻿using System;
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

        public void Sync(ICrmService crm)
        {
            SyncTeachingEvents(crm);
            SyncPrivacyPolicies(crm);
            SyncTypeEntities(crm);
        }

        public IQueryable<TypeEntity> GetLookupItems(string entityName)
        {
            return _dbContext.TypeEntities.Where(t => t.EntityName == entityName);
        }

        public IQueryable<TypeEntity> GetPickListItems(string entityName, string attributeName)
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
                teachingEvents = teachingEvents.Where(te => te.TypeId == request.TypeId);

            if (request.StartAfter != null)
                teachingEvents = teachingEvents.Where(te => request.StartAfter < te.StartAt);

            if (request.StartBefore != null)
                teachingEvents = teachingEvents.Where(te => request.StartBefore > te.StartAt);

            if (request.Radius == null)
                return await teachingEvents.ToListAsync();

            return await FilterTeachingEventsByRadius(teachingEvents, request);
        }

        public async Task<TeachingEvent> GetTeachingEventAsync(Guid id)
        {
            return await _dbContext.TeachingEvents.FirstOrDefaultAsync(teachingEvent => teachingEvent.Id == id);
        }

        public bool IsValidPostcode(string postcode)
        {
            if (string.IsNullOrEmpty(postcode))
                return false;

            return _dbContext.Locations.Any(l => l.Postcode == Location.SanitizePostcode(postcode));
        }

        private async Task<IEnumerable<TeachingEvent>> FilterTeachingEventsByRadius(
            IQueryable<TeachingEvent> teachingEvents, TeachingEventSearchRequest request)
        {
            var origin = CoordinateForPostcode(request.Postcode);

            // Exclude events we don't have a location for.
            teachingEvents = teachingEvents.Where(te => te.Building != null && te.Building.Coordinate != null);

            // Approximate distance filtering in the database, with a suitable error margin (treats distance as an arc degree).
            teachingEvents = teachingEvents.Where(te => EarthCircumferenceInKm *
                te.Building.Coordinate.Distance(origin) / 360 < request.RadiusInKm + ErrorMarginInKm);

            var inMemoryTeachingEvents = await teachingEvents.ToListAsync();

            // Project coordinates onto UK coordinate system for final, accurate distance filtering.
            return inMemoryTeachingEvents.Where(te => te.Building.Coordinate.ProjectTo(DbConfiguration.UkSrid)
                    .IsWithinDistance(origin.ProjectTo(DbConfiguration.UkSrid), (double) request.RadiusInKm * 1000));
        }

        private void SyncTeachingEvents(ICrmService crm)
        {
            var teachingEvents = crm.GetTeachingEvents().ToList();
            PopulateTeachingEventCoordinates(teachingEvents);

            var buildings = teachingEvents.Where(te => te.Building != null)
                .Select(te => te.Building).DistinctBy(b => b.Id);

            SyncModels(buildings, _dbContext.TeachingEventBuildings);

            // Link events with buildings attached to the context prior to sync.
            teachingEvents.Where(te => te.Building != null).ToList()
                .ForEach(te => te.Building = _dbContext.TeachingEventBuildings.Find(te.Building.Id));

            SyncModels(teachingEvents, _dbContext.TeachingEvents);

            _dbContext.SaveChanges();
        }

        private void SyncPrivacyPolicies(ICrmService crm)
        {
            var policies = crm.GetPrivacyPolicies().ToList();
            SyncModels(policies, _dbContext.PrivacyPolicies);
        }

        private void SyncTypeEntities(ICrmService crm)
        {
            SyncTypes(crm.GetLookupItems("dfe_country"));
            SyncTypes(crm.GetLookupItems("dfe_teachingsubjectlist"));
            SyncTypes(crm.GetPickListItems("contact", "dfe_ittyear"));
            SyncTypes(crm.GetPickListItems("contact", "dfe_preferrededucationphase01"));
            SyncTypes(crm.GetPickListItems("contact", "dfe_isinuk"));
            SyncTypes(crm.GetPickListItems("contact", "dfe_channelcreation"));
            SyncTypes(crm.GetPickListItems("dfe_qualification", "dfe_degreestatus"));
            SyncTypes(crm.GetPickListItems("dfe_qualification", "dfe_category"));
            SyncTypes(crm.GetPickListItems("dfe_qualification", "dfe_type"));
            SyncTypes(crm.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase"));
            SyncTypes(crm.GetPickListItems("msevtmgt_event", "dfe_event_type"));
            SyncTypes(crm.GetPickListItems("phonecall", "dfe_channelcreation"));
        }

        private void SyncModels<T>(IEnumerable<T> models, IQueryable<T> dbSet) where T : BaseModel
        {
            var existingIds = dbSet.Select(m => m.Id);
            var modelIds = models.Select(m => m.Id);

            _dbContext.RemoveRange(dbSet.Where(m => !modelIds.Contains(m.Id)));
            _dbContext.UpdateRange(models.Where(m => existingIds.Contains(m.Id)));
            _dbContext.AddRange(models.Where(m => !existingIds.Contains(m.Id)));
            _dbContext.SaveChanges();
        }

        private void SyncTypes(IEnumerable<TypeEntity> types)
        {
            if (!types.Any()) return;

            var key = types.Select(t => new { t.EntityName, t.AttributeName }).First();
            var typeIds = types.Select(t => t.Id);
            var existingIds = _dbContext.TypeEntities
                .Where(t => t.EntityName == key.EntityName && t.AttributeName == key.AttributeName)
                .Select(t => t.Id);

            _dbContext.RemoveRange(_dbContext.TypeEntities.Where(t => t.EntityName == key.EntityName 
                && t.AttributeName == key.AttributeName && !typeIds.Contains(t.Id)));
            _dbContext.UpdateRange(types.Where(t => existingIds.Contains(t.Id)));
            _dbContext.AddRange(types.Where(t => !existingIds.Contains(t.Id)));
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
