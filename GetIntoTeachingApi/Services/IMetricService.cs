﻿using Prometheus;

namespace GetIntoTeachingApi.Services
{
    public interface IMetricService
    { 
        Histogram CrmSyncDuration { get; }
    }
}
