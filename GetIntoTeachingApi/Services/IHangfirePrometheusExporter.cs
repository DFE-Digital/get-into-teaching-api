namespace GetIntoTeachingApi.Services
{
    public interface IHangfirePrometheusExporter
    {
        void ExportHangfireStatistics();
    }
}
