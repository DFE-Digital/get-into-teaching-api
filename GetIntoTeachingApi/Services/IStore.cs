using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public interface IStore
    {
        void Sync(ICrmService crm);
        IQueryable<TypeEntity> GetLookupItems(string entityName);
        IQueryable<TypeEntity> GetPickListItems(string entityName, string attributeName);
        PrivacyPolicy GetLatestPrivacyPolicy();
        IEnumerable<PrivacyPolicy> GetPrivacyPolicies();
        IQueryable<TeachingEvent> GetUpcomingTeachingEvents(int limit);
        Task<IEnumerable<TeachingEvent>> SearchTeachingEventsAsync(TeachingEventSearchRequest request);
        Task<TeachingEvent> GetTeachingEventAsync(Guid id);
        bool IsValidPostcode(string postcode);
    }
}
