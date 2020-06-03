using System;
using GetIntoTeachingApi.Adapters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetIntoTeachingApi.Utils;

namespace GetIntoTeachingApi.Services
{
    public class NotifyService : INotifyService
    {
        public static readonly string NewPinCodeEmailTemplateId = "f974aa10-f3a6-450d-87ca-8757644335fc";
        public static readonly string CandidateRegistrationFailedTemplateId = "00ea3516-17b0-4e09-8a92-ddec606310fd";
        public static readonly string TeachingEventRegistrationFailedTemplateId = "b4084e28-60a6-417d-bd66-42112bd7ad09";
        private readonly ILogger<NotifyService> _logger;
        private readonly INotificationClientAdapter _client;
        private readonly IEnv _env;

        public NotifyService(ILogger<NotifyService> logger, INotificationClientAdapter client, IEnv env)
        {
            _logger = logger;
            _client = client;
            _env = env;
        }

        public Task SendEmail(string email, string templateId, Dictionary<string, dynamic> personalisation)
        {
            return _client.SendEmailAsync(
                ApiKey(),
                email,
                templateId,
                personalisation
            ).ContinueWith(task => _logger.LogWarning($"NotifyService - Failed to send email: {task.Exception?.Message}"), 
                TaskContinuationOptions.OnlyOnFaulted);
        }

        private string ApiKey() => _env.NotifyApiKey;
    }
}
