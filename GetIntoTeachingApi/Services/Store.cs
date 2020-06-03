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
        private const double EarthCircumferenceInMeters = 40075.017;
        private const double ErrorMarginInMeters = 16093.4;
        private readonly GetIntoTeachingDbContext _dbContext;

        public Store(GetIntoTeachingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Sync(ICrmService crm)
        {
            SyncTeachingEvents(crm);
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
                teachingEvents = teachingEvents.Where(te => EarthCircumferenceInMeters * 
                    te.Building.Coordinate.Distance(origin) / 360 < request.RadiusInMeters + ErrorMarginInMeters);

                // Project coordinates in-memory for accurate distance filtering.
                teachingEvents = teachingEvents.ToList().Where(te => 
                    te.Building.Coordinate.ProjectTo(DbConfiguration.UkSrid)
                        .IsWithinDistance(origin.ProjectTo(DbConfiguration.UkSrid), 
                            (double) request.RadiusInMeters));
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

            UpsertTeachingEventBuildings(teachingEvents);
            UpsertTeachingEvents(teachingEvents);

            _dbContext.SaveChanges();
        }

        private void UpsertTeachingEventBuildings(IEnumerable<TeachingEvent> teachingEvents)
        {
            var buildings = teachingEvents.Where(te => te.Building != null).Select(te => te.Building).DistinctBy(b => b.Id);
            var buildingIds = buildings.Select(b => b.Id);
            var existingBuildingIds = _dbContext.TeachingEventBuildings.Where(b => buildingIds.Contains(b.Id)).Select(b => b.Id);
            _dbContext.UpdateRange(buildings.Where(b => existingBuildingIds.Contains(b.Id)));
            _dbContext.AddRange(buildings.Where(b => !existingBuildingIds.Contains(b.Id)));
        }

        private void UpsertTeachingEvents(IEnumerable<TeachingEvent> teachingEvents)
        {
            var teachingEventIds = teachingEvents.Select(te => te.Id);
            var existingTeachingEventIds = _dbContext.TeachingEvents.Where(te => teachingEventIds.Contains(te.Id)).Select(te => te.Id);
            teachingEvents.Where(te => te.Building != null).ToList().ForEach(te => te.Building = _dbContext.TeachingEventBuildings.Find(te.Building.Id));
            _dbContext.UpdateRange(teachingEvents.Where(te => existingTeachingEventIds.Contains(te.Id)));
            _dbContext.AddRange(teachingEvents.Where(te => !existingTeachingEventIds.Contains(te.Id)));
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
