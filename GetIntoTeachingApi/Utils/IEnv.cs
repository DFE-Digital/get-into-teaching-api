﻿namespace GetIntoTeachingApi.Utils
{
    public interface IEnv
    {
        bool IsDevelopment { get; }
        bool IsStaging { get; }
        bool IsProduction { get; }
        bool IsTest { get; }
        bool ExportHangireToPrometheus { get; }
        string InstanceIndex { get; }
        string GitCommitSha { get; }
        string DatabaseInstanceName { get; }
        string PgConnectionString { get; }
        string HangfireInstanceName { get; }
        string HangfireUsername { get; }
        string HangfirePassword { get; }
        string EnvironmentName { get; }
        string CloudFoundryEnvironmentName { get; }
        string TotpSecretKey { get; }
        string VcapServices { get; }
        string AppName { get; }
        string Organization { get; }
        string Space { get; }
        string CrmServiceUrl { get; }
        string CrmClientId { get; }
        string CrmClientSecret { get; }
        string ApplyCandidateApiUrl { get; }
        string ApplyCandidateApiKey { get; }
        string NotifyApiKey { get; }
        string GoogleApiKey { get; }
        bool IsMasterInstance { get; }

        bool IsFeatureOn(string feature);
        bool IsFeatureOff(string feature);
        string GetVariable(string variable);
    }
}
