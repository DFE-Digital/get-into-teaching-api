﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;

namespace GetIntoTeachingApi.Services
{
    public interface IStore
    {
        Task<string> CheckStatusAsync();
        Task SyncAsync();
        Task SaveAsync<T>(IEnumerable<T> models)
            where T : BaseModel;
        Task SaveAsync<T>(T model)
            where T : BaseModel;
        IQueryable<Country> GetCountries();
        IQueryable<TeachingSubject> GetTeachingSubjects();
        IQueryable<PickListItem> GetPickListItems(string entityName, string attributeName);
        Task<PrivacyPolicy> GetLatestPrivacyPolicyAsync();
        IQueryable<PrivacyPolicy> GetPrivacyPolicies();
        Task<PrivacyPolicy> GetPrivacyPolicyAsync(Guid id);
        Task<IEnumerable<TeachingEvent>> SearchTeachingEventsAsync(TeachingEventSearchRequest request);
        Task<TeachingEvent> GetTeachingEventAsync(Guid id);
        Task<TeachingEvent> GetTeachingEventAsync(string readableId);
        IQueryable<TeachingEventBuilding> GetTeachingEventBuildings();
    }
}
