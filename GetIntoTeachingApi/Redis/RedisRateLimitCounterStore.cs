using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using GetIntoTeachingApi.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace GetIntoTeachingApi.Redis
{
    public class RedisRateLimitCounterStore : IRateLimitCounterStore
    {
        private readonly ILogger<RedisRateLimitCounterStore> _logger;
        private readonly IRateLimitCounterStore _memoryCacheStore;
        private readonly ConnectionMultiplexer _redis;

        public RedisRateLimitCounterStore(
            IEnv env,
            IMemoryCache memoryCache,
            ILogger<RedisRateLimitCounterStore> logger)
        {
            _logger = logger;
            _memoryCacheStore = new MemoryCacheRateLimitCounterStore(memoryCache);
            var options = RedisConfiguration.ConfigurationOptions(env);
            _redis = ConnectionMultiplexer.Connect(options);
        }

        private IDatabase RedisDatabase => _redis.GetDatabase();

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await TryRedisCommandAsync(
                () =>
                {
                    return RedisDatabase.KeyExistsAsync(id);
                },
                () =>
                {
                    return _memoryCacheStore.ExistsAsync(id, cancellationToken);
                });
        }

        public async Task<RateLimitCounter?> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await TryRedisCommandAsync(
                async () =>
                {
                    var value = await RedisDatabase.StringGetAsync(id);

                    if (!string.IsNullOrEmpty(value))
                    {
                        return JsonConvert.DeserializeObject<RateLimitCounter?>(value);
                    }

                    return null;
                },
                () =>
                {
                    return _memoryCacheStore.GetAsync(id, cancellationToken);
                });
        }

        public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _ = await TryRedisCommandAsync(
                async () =>
                {
                    await RedisDatabase.KeyDeleteAsync(id);

                    return true;
                },
                async () =>
                {
                    await _memoryCacheStore.RemoveAsync(id, cancellationToken);

                    return true;
                });
        }

        public async Task SetAsync(string id, RateLimitCounter? entry, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _ = await TryRedisCommandAsync(
                async () =>
                {
                    await RedisDatabase.StringSetAsync(id, JsonConvert.SerializeObject(entry.Value), expirationTime);

                    return true;
                },
                async () =>
                {
                    await _memoryCacheStore.SetAsync(id, entry, expirationTime, cancellationToken);

                    return true;
                });
        }

        private async Task<T> TryRedisCommandAsync<T>(Func<Task<T>> command, Func<Task<T>> fallbackCommand)
        {
            if (_redis?.IsConnected == true)
            {
                try
                {
                    return await command();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Redis command failed: {ex}");
                }
            }

            return await fallbackCommand();
        }
    }
}