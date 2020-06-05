using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public interface IStore
    {
        void Sync(ICrmService crm);
        public IEnumerable<TypeEntity> GetLookupItems(string entityName);
        public IEnumerable<TypeEntity> GetPickListItems(string entityName, string attributeName);
        public PrivacyPolicy GetLatestPrivacyPolicy();
        public IEnumerable<PrivacyPolicy> GetPrivacyPolicies();
        IEnumerable<TeachingEvent> GetUpcomingTeachingEvents(int limit);
        IEnumerable<TeachingEvent> SearchTeachingEvents(TeachingEventSearchRequest request);
        TeachingEvent GetTeachingEvent(Guid id);
        bool IsValidPostcode(string postcode);
    }
}
