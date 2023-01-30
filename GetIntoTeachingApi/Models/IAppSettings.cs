using System;

namespace GetIntoTeachingApi.Models
{
    public interface IAppSettings
    {
        DateTime? ApplyLastSyncAt { get; set; }
        DateTime? CrmIntegrationPausedUntil { get; set; }
        bool IsApplyBackfillInProgress { get; set; }
        bool IsCrmIntegrationPaused { get; }
    }
}
