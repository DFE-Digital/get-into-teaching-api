using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Location = GetIntoTeachingApi.Models.Location;

namespace GetIntoTeachingApi.Services
{
    public class Store : IStore
    {
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
            var teachingEvents = _dbContext.TeachingEvents.Include(te => te.Building).AsQueryable();

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
                teachingEvents = teachingEvents.Where(te => te.Building.Coordinate
                    .IsWithinDistance(origin, MilesToDegrees((double)request.Radius)));
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

            var ids = teachingEvents.Select(te => te.Id);
            var existingTeachingEventIds = _dbContext.TeachingEvents
                .Where(te => ids.Contains(te.Id)).Select(te => te.Id);

            _dbContext.UpdateRange(teachingEvents.Where(te => existingTeachingEventIds.Contains(te.Id)));
            _dbContext.AddRange(teachingEvents.Where(te => !existingTeachingEventIds.Contains(te.Id)));

            _dbContext.SaveChanges();
        }

        private void PopulateTeachingEventCoordinates(IEnumerable<TeachingEvent> teachingEvents)
        {
            foreach (var teachingEvent in teachingEvents.Where(te => te.Building?.AddressPostcode != null))
            {
                teachingEvent.Building.Coordinate = CoordinateForPostcode(teachingEvent.Building.AddressPostcode);
            }
        }

        private static double MilesToDegrees(double miles)
        { 
            const double metersInAMile = 1609.34;
            // EarthMeanRadius * PI / 180
            // (within 1% accuracy to the geodesic distance for the WGS84 ellipsoid).
            const double metersPerDegree = 111195;

            return (miles * metersInAMile) / metersPerDegree;
        }

        private Point CoordinateForPostcode(string postcode)
        {
            var sanitizedPostcode = Location.SanitizePostcode(postcode);
            var location = _dbContext.Locations.FirstOrDefault(l => l.Postcode == sanitizedPostcode);

            return location?.Coordinate;
        }
    }
}
