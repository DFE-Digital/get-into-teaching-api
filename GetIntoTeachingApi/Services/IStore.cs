using System;
using System.Collections.Generic;
using System.Linq;
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
        IEnumerable<TeachingEvent> GetUpcomingTeachingEvents(int limit);
        IEnumerable<TeachingEvent> SearchTeachingEvents(TeachingEventSearchRequest request);
        TeachingEvent GetTeachingEvent(Guid id);
        bool IsValidPostcode(string postcode);
    }
}
