﻿namespace GetIntoTeachingApi.Utils
{
    public interface IEnv
    {
        bool IsDevelopment { get; }
        bool IsStaging { get; }
        bool IsProduction { get; }
        bool ExportHangireToPrometheus { get; }
        string EnvironmentName { get; }
        string TotpSecretKey { get; }
        string VcapServices { get; }
        string CrmServiceUrl { get; }
        string CrmClientId { get; }
        string CrmClientSecret { get; }
        string NotifyApiKey { get; }
        string SharedSecret { get; }
    }
}
