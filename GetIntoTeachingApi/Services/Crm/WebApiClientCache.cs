using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Services.Crm
{
    public class WebApiClientCache : IWebApiClientCache
    {
        private readonly IDictionary<string, CacheEntry> _store;
        private readonly ILogger<WebApiClientCache> _logger;

        public WebApiClientCache(ILogger<WebApiClientCache> logger)
        {
            _store = new Dictionary<string, CacheEntry>();
            _logger = logger;
        }

        public async Task<TItem> GetOrCreateAsync<TItem>(string key, DateTime expiresAt, Func<Task<TItem>> create)
        {
            await CreateEntryIfNew(key, expiresAt, create);
            await RefreshEntryIfExpired(key, expiresAt, create);

            return (TItem)_store[key].Entry;
        }

        private async Task CreateEntryIfNew<TItem>(string key, DateTime expiresAt, Func<Task<TItem>> create)
        {
            if (_store.ContainsKey(key)) return;

            _store[key] = new CacheEntry((await create()), expiresAt);
        }

        private async Task RefreshEntryIfExpired<TItem>(string key, DateTime expiresAt, Func<Task<TItem>> create)
        {
            if (!_store.ContainsKey(key) || !_store[key].Expired()) return;

            try
            {
                _store[key] = new CacheEntry((await create()), expiresAt);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"WebApiClientCache - Failed to refresh cache ({key}): {e.Message}");
            }
        }

        internal class CacheEntry
        {
            public DateTime ExpiresAt { get; set; }
            public dynamic Entry { get; set; }

            public CacheEntry(dynamic entry, DateTime expiresAt)
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
}
