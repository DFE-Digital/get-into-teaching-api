using Notify.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Adapters
{
    public class NotificationClientAdapter : INotificationClientAdapter
    {
        private readonly IDictionary<string, NotificationClient> _clients;

        public NotificationClientAdapter()
        {
            _clients = new Dictionary<string, NotificationClient>();
        }

        public Task SendEmailAsync(string apiKey, string email, string templateId, Dictionary<string, dynamic> personalisation)
        {
            return Client(apiKey).SendEmailAsync(email, templateId, personalisation);
        }

        private NotificationClient Client(string apiKey)
        {
            if (!_clients.ContainsKey(apiKey))
            {
                _clients[apiKey] = new NotificationClient(apiKey);
            }

            return _clients[apiKey];
        }
    }
}
