using System;

namespace GetIntoTeachingApi.Models
{
    public interface IAppSettings
    {
        DateTime? CrmIntegrationPausedUntil { get; set; }
        bool IsCrmIntegrationPaused { get; }
    }
}
