using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Utils;
using StackExchange.Redis;

namespace GetIntoTeachingApi.Redis
{
    public static class RedisConfiguration
    {
        private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        public static ConfigurationOptions ConfigurationOptions(IEnv env)
        {
            var vcap = JsonSerializer.Deserialize<VcapServices>(env.VcapServices, _jsonSerializerOptions);
            var redis = vcap.Redis.First();
            var credentials = redis.Credentials;

            return new ConfigurationOptions()
            {
                EndPoints = { { credentials.Host, credentials.Port } },
                Password = credentials.Password,
                AbortOnConnectFail = false,
                Ssl = credentials.TlsEnabled,
            };
        }

        internal sealed class VcapServices
        {
            public IEnumerable<VcapRedis> Redis { get; set; }
        }

        internal sealed class VcapRedis
        {
            public VcapCredentials Credentials { get; set; }
        }

        internal sealed class VcapCredentials
        {
            public string Host { get; set; }
            public string Password { get; set; }
            public int Port { get; set; }
            [JsonPropertyName("tls_enabled")]
            public bool TlsEnabled { get; set; }
        }
    }
}
