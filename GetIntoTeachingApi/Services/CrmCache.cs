using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Services
{
    public class CrmCache : ICrmCache
    {
        private readonly IDictionary<string, CacheEntry> _store;
        private readonly ILogger<CrmCache> _logger;

        public CrmCache(ILogger<CrmCache> logger)
        {
            _store = new Dictionary<string, CacheEntry>();
            _logger = logger;
        }

        public TItem GetOrCreate<TItem>(string key, DateTime expiresAt, Func<TItem> create)
        {
            CreateEntryIfNew(key, expiresAt, create);
            RefreshEntryIfExpired(key, expiresAt, create);

            return (TItem) _store[key].Entry;
        }

        private void CreateEntryIfNew<TItem>(string key, DateTime expiresAt, Func<TItem> create)
        {
            if (_store.ContainsKey(key)) return;
            
            _store[key] = new CacheEntry(create(), expiresAt);
        }

        private void RefreshEntryIfExpired<TItem>(string key, DateTime expiresAt, Func<TItem> create)
        {
            if (!_store.ContainsKey(key) || !_store[key].Expired()) return;

            try
            {
                _store[key] = new CacheEntry(create(), expiresAt);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"CrmCache - Failed to refresh cache ({key}): {e.Message}");
            }
        }
    }

    internal class CacheEntry
    {
        public DateTime ExpiresAt { get; set; }
        public object Entry { get; set; }

        public CacheEntry(object entry, DateTime expiresAt)
        {
            Entry = entry;
            ExpiresAt = expiresAt;
        }

        public bool Expired()
        {
            return DateTime.Now > ExpiresAt;
        }
    }
}
