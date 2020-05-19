using GetIntoTeachingApi.Adapters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Services
{
    public class NotifyService : INotifyService
    {
        public static readonly string NewPinCodeEmailTemplateId = "f974aa10-f3a6-450d-87ca-8757644335fc";
        private readonly ILogger<NotifyService> _logger;
        private readonly INotificationClientAdapter _client;

        public NotifyService(ILogger<NotifyService> logger, INotificationClientAdapter client)
        {
            _logger = logger;
            _client = client;
        }

        public void SendEmail(string email, string templateId, Dictionary<string, dynamic> personalisation)
        {
            _client.SendEmailAsync(
                ApiKey(),
                email,
                templateId,
                personalisation
            ).ContinueWith(task => _logger.LogWarning($"NotifyService - Failed to send email: {task.Exception?.Message}"), 
                TaskContinuationOptions.OnlyOnFaulted);
        }

        private static string ApiKey()
        {
            return Environment.GetEnvironmentVariable("NOTIFY_API_KEY");
        }
    }
}
