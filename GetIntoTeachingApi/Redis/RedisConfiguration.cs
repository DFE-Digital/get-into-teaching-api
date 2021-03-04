using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Utils;
using StackExchange.Redis;

namespace GetIntoTeachingApi.Redis
{
    public class RedisConfiguration
    {
        public static ConfigurationOptions ConfigurationOptions(IEnv env)
        {
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var vcap = JsonSerializer.Deserialize<VcapServices>(env.VcapServices, options);
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

        internal class VcapServices
        {
            public IEnumerable<VcapRedis> Redis { get; set; }
        }

        internal class VcapRedis
        {
            public VcapCredentials Credentials { get; set; }
        }

        internal class VcapCredentials
        {
            public string Host { get; set; }
            public string Password { get; set; }
            public int Port { get; set; }
            [JsonPropertyName("tls_enabled")]
            public bool TlsEnabled { get; set; }
        }
    }
}
