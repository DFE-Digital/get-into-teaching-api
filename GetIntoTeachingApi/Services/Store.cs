using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GetIntoTeachingApi.Services
{
    public class Store : IStore
    {
        private readonly GetIntoTeachingDbContext _dbContext;
        private readonly ILocationService _locationService;

        public Store(GetIntoTeachingDbContext dbContext, ILocationService locationService)
        {
            _dbContext = dbContext;
            _locationService = locationService;
        }

        public void Sync(ICrmService crm)
        {
            var teachingEvents = crm.GetTeachingEvents();
            _dbContext.UpsertRange(teachingEvents).On(m => m.Id).Run();
            _dbContext.SaveChanges();
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
            return _dbContext.TeachingEvents
                .ToList()
                .Where((teachingEvent) => request.Match(teachingEvent, _locationService))
                .OrderBy(teachingEvent => teachingEvent.StartAt);
        }

        public TeachingEvent GetTeachingEvent(Guid id)
        {
            return _dbContext.TeachingEvents.FirstOrDefault(teachingEvent => teachingEvent.Id == id);
        }
    }
}
