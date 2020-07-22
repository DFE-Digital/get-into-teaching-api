using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public interface IStore
    {
        Task<string> CheckStatusAsync();
        Task SyncAsync(ICrmService crm);
        IQueryable<TypeEntity> GetLookupItems(string entityName);
        IQueryable<TypeEntity> GetPickListItems(string entityName, string attributeName);
        Task<PrivacyPolicy> GetLatestPrivacyPolicyAsync();
        IQueryable<PrivacyPolicy> GetPrivacyPolicies();
        Task<PrivacyPolicy> GetPrivacyPolicyAsync(Guid id);
        IQueryable<TeachingEvent> GetUpcomingTeachingEvents(int limit);
        Task<IEnumerable<TeachingEvent>> SearchTeachingEventsAsync(TeachingEventSearchRequest request);
        Task<TeachingEvent> GetTeachingEventAsync(Guid id);
        Task<TeachingEvent> GetTeachingEventAsync(string readableId);
        bool IsValidPostcode(string postcode);
    }
}
