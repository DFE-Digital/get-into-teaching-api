using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Adapters
{
    public interface INotificationClientAdapter
    {
        public Task SendEmailAsync(string apiKey, string email, string templateId, Dictionary<string, dynamic> personalisation);
    }
}
