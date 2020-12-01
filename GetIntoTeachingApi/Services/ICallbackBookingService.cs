using System.Collections.Generic;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public interface ICallbackBookingService
    {
        IEnumerable<CallbackBookingQuota> GetCallbackBookingQuotas();
    }
}
