using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Utils;
using Prometheus;

namespace GetIntoTeachingApi.AppStart
{
    public static class PrometheusMetrics
    {
        public static void Configure()
        {
            // See: https://github.com/prometheus-net/prometheus-net/issues/407
            Metrics.DefaultFactory.ExemplarBehavior = ExemplarBehavior.NoExemplars();
        }

        public static void SetStaticLabels(IEnv env)
        {
            if (!Metrics.DefaultRegistry.StaticLabels.Any())
            {
                Metrics.DefaultRegistry.SetStaticLabels(new Dictionary<string, string>
                {
                    { "app", env.AppName },
                    { "organization", env.Organization },
                    { "space", env.Space },
                    { "app_instance", env.InstanceIndex },
                });
            }
        }
    }
}