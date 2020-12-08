using System;
using System.Threading.Tasks;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Utils;
using StackExchange.Redis;

namespace GetIntoTeachingApi.Services
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly ConfigurationOptions _options;

        public RedisService(IEnv env)
        {
            _options = RedisConfiguration.ConfigurationOptions(env);
            _redis = ConnectionMultiplexer.Connect(_options);
        }

        public async Task<string> CheckStatusAsync()
        {
            try
            {
                await _redis.GetServer(_options.EndPoints[0]).PingAsync();
                return HealthCheckResponse.StatusOk;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
