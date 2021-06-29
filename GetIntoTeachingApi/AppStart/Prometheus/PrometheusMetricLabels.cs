using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Utils;
using Prometheus;

namespace GetIntoTeachingApi.AppStart.Prometheus
{
    public static class PrometheusMetricLabels
    {
        public static void SetLabels(IEnv env)
        {
            if (!Metrics.DefaultRegistry.StaticLabels.Any())
            {
                Metrics.DefaultRegistry.SetStaticLabels(new Dictionary<string, string>
                {
                    { "app", env.AppName },
                    { "organization", env.Organization },
                    { "space", env.Space },
                });
            }
        }
    }
}
