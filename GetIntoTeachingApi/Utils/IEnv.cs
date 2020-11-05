namespace GetIntoTeachingApi.Utils
{
    public interface IEnv
    {
        bool IsDevelopment { get; }
        bool IsStaging { get; }
        bool IsProduction { get; }
        bool IsTest { get; }
        bool ExportHangireToPrometheus { get; }
        string GitCommitSha { get; }
        string DatabaseInstanceName { get; }
        string HangfireInstanceName { get; }
        string EnvironmentName { get; }
        string TotpSecretKey { get; }
        string VcapServices { get; }
        string CrmServiceUrl { get; }
        string CrmClientId { get; }
        string CrmClientSecret { get; }
        string NotifyApiKey { get; }
        string SharedSecret { get; }
        string PenTestSharedSecret { get; }
        string GoogleApiKey { get; }
        int InstanceIndex { get; }
    }
}
