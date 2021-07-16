using System.Collections.Generic;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApi.Services
{
    public interface ICallbackBookingService
    {
        IEnumerable<CallbackBookingQuota> GetCallbackBookingQuotas();
    }
}
