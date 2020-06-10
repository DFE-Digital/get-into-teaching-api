using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Services
{
    public interface INotifyService
    {
        public Task SendEmailAsync(string email, string templateId, Dictionary<string, dynamic> personalisation);
    }
}
