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
        IQueryable<LookupItem> GetLookupItems(string entityName);
        IQueryable<TypeEntity> GetTypeEntitites(string entityName, string attributeName = null);
        Task<PrivacyPolicy> GetLatestPrivacyPolicyAsync();
        IQueryable<PrivacyPolicy> GetPrivacyPolicies();
        Task<PrivacyPolicy> GetPrivacyPolicyAsync(Guid id);
        Task<IEnumerable<TeachingEvent>> SearchTeachingEventsAsync(TeachingEventSearchRequest request);
        Task<TeachingEvent> GetTeachingEventAsync(Guid id);
        Task<TeachingEvent> GetTeachingEventAsync(string readableId);
        IQueryable<TeachingEvent> GetUpcomingTeachingEvents();
    }
}
