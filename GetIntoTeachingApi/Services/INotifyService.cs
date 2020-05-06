using System.Collections.Generic;

namespace GetIntoTeachingApi.Services
{
    public interface INotifyService
    {
        public void SendEmail(string email, string templateId, Dictionary<string, dynamic> personalisation);
    }
}
