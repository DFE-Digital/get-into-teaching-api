using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetIntoTeachingApi.Models;
using Notify.Client;

namespace GetIntoTeachingApi.Adapters
{
    public class NotificationClientAdapter : INotificationClientAdapter
    {
        private readonly IDictionary<string, NotificationClient> _clients;

        public NotificationClientAdapter()
        {
            _clients = new Dictionary<string, NotificationClient>();
        }

        public async Task<string> CheckStatusAsync(string apiKey)
        {
            try
            {
                var client = Client(apiKey);
                await client.GetAllTemplatesAsync();
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return HealthCheckResponse.StatusOk;
        }

        public Task SendEmailAsync(string apiKey, string email, string templateId, Dictionary<string, dynamic> personalisation)
        {
            return Client(apiKey).SendEmailAsync(email, templateId, personalisation);
        }

        private NotificationClient Client(string apiKey)
        {
            if (!_clients.TryGetValue(apiKey, out NotificationClient value))
            {
                value = new NotificationClient(apiKey);
                _clients[apiKey] = value;
            }
            return value;
        }
    }
}
