using System.Collections.Generic;
using System.IO;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Newtonsoft.Json;

namespace GetIntoTeachingApiTests.Helpers
{
    public class ContractCallbackBookingService : ICallbackBookingService
    {
        public IEnumerable<CallbackBookingQuota> GetCallbackBookingQuotas()
        {
            var json = File.ReadAllText("./Contracts/Data/callback_booking_quotas.json");
            return JsonConvert.DeserializeObject<IEnumerable<CallbackBookingQuota>>(json);
        }
    }
}
