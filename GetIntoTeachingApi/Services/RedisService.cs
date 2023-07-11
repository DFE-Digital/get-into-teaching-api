using System;
using System.Threading.Tasks;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Redis;
using GetIntoTeachingApi.Utils;
using StackExchange.Redis;
using System.Net;

namespace GetIntoTeachingApi.Services
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly ConfigurationOptions _options;
        private readonly EndPoint _endpoint;

        public IDatabase Database => _redis.GetDatabase();

        public RedisService(IEnv env)
        {
          if (string.IsNullOrEmpty(env.RedisConnectionString)) {
            _options = RedisConfiguration.ConfigurationOptions(env);
            _endpoint = _options.EndPoints[0];
            _redis = ConnectionMultiplexer.Connect(_options);
          } else {
            _redis = ConnectionMultiplexer.Connect(env.RedisConnectionString);
            _endpoint = _redis.GetEndPoints()[0];
          }
        }

        public async Task<string> CheckStatusAsync()
        {
            try
            {
                await _redis.GetServer(_endpoint).PingAsync();
                return HealthCheckResponse.StatusOk;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
