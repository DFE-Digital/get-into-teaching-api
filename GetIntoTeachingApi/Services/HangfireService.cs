using System;
using GetIntoTeachingApi.Models;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace GetIntoTeachingApi.Services
{
    public class HangfireService : IHangfireService
    {
        private readonly ILogger<HangfireService> _logger;
        private readonly IBackgroundJobClient _jobClient;

        public HangfireService(IBackgroundJobClient jobClient, ILogger<HangfireService> logger)
        {
            _logger = logger;
            _jobClient = jobClient;
        }

        public string CheckStatus()
        {
            try
            {
                _jobClient.Enqueue(() => _logger.LogInformation("Hangfire - Status Check"));
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return HealthCheckResponse.StatusOk;
        }
    }
}
