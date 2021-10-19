using System;

namespace GetIntoTeachingApi.Models
{
    public interface IAppSettings
    {
        DateTime? FindApplyLastSyncAt { get; set; }
        DateTime? CrmIntegrationPausedUntil { get; set; }
        bool IsFindApplyBackfillInProgress { get; set; }
        bool IsCrmIntegrationPaused { get; }
    }
}
