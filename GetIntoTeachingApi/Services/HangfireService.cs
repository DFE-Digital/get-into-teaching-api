using System;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using Hangfire;

namespace GetIntoTeachingApi.Services
{
    public class HangfireService : IHangfireService
    {
        private readonly IBackgroundJobClient _jobClient;

        public HangfireService(IBackgroundJobClient jobClient)
        {
            _jobClient = jobClient;
        }

        public string CheckStatus()
        {
            try
            {
                _jobClient.Enqueue<StatusCheckJob>((x) => x.Run());
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return HealthCheckResponse.StatusOk;
        }
    }
}
