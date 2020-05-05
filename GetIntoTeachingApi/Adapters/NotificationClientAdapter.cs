using Notify.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Adapters
{
    public class NotificationClientAdapter : INotificationClientAdapter
    {
        public Task SendEmailAsync(string apiKey, string email, string templateId, Dictionary<string, dynamic> personalisation)
        {
            var client = new NotificationClient(apiKey);
            return client.SendEmailAsync(email, templateId, personalisation);
        }
    }
}
