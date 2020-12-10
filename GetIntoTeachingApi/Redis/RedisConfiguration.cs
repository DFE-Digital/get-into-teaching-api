using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Utils;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace GetIntoTeachingApi.Redis
{
    public class RedisConfiguration
    {
        public static ConfigurationOptions ConfigurationOptions(IEnv env)
        {
            var vcap = JsonConvert.DeserializeObject<VcapServices>(env.VcapServices);
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
            [JsonProperty("tls_enabled")]
            public bool TlsEnabled { get; set; }
        }
    }
}
