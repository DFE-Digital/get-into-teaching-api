namespace GetIntoTeachingApi.Utils
{
    public interface IEnv
    {
        public string EnvironmentName { get; }
        public string TotpSecretKey { get; }
        public string VcapServices { get; }
        public string CrmServiceUrl { get; }
        public string CrmClientId { get; }
        public string CrmClientSecret { get; }
        public string NotifyApiKey { get; }
        public string SharedSecret { get; }
    }
}
