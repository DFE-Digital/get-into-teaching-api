using System;
using System.Linq;
using GetIntoTeachingApi.Models;
using Hangfire;

namespace GetIntoTeachingApi.Services
{
    public class HangfireService : IHangfireService
    {
        private readonly JobStorage _jobStorage;

        public HangfireService(JobStorage jobStorage)
        {
            _jobStorage = jobStorage;
        }

        public string CheckStatus()
        {
            try
            {
                var servers = _jobStorage.GetMonitoringApi().Servers();
                bool queueIsBeingProcessed = servers.Any(
                    s => s.Queues.Contains("Default", StringComparer.InvariantCultureIgnoreCase));

                return queueIsBeingProcessed ? HealthCheckResponse.StatusOk : "No workers are processing the Default queue!";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
