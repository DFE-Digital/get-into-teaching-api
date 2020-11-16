using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using CsvHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prometheus;

namespace GetIntoTeachingApi.Jobs
{
    public class LocationSyncJob : BaseJob
    {
        public const string UkPostcodeCsvFilename = "ukpostcodes.csv";
        private const int BatchInterval = 100;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger<LocationSyncJob> _logger;
        private readonly IMetricService _metrics;

        public LocationSyncJob(
            IEnv env,
            IBackgroundJobClient jobClient,
            ILogger<LocationSyncJob> logger,
            IMetricService metrics)
            : base(env)
        {
            _logger = logger;
            _jobClient = jobClient;
            _metrics = metrics;
        }

        public async Task RunAsync(string ukPostcodeCsvUrl)
        {
            using (_metrics.LocationSyncDuration.NewTimer())
            {
                _logger.LogInformation($"LocationSyncJob - Started");

                var csvPath = await RetrieveCsv(ukPostcodeCsvUrl);

                try
                {
                    await SyncLocations(csvPath);
                }
                finally
                {
                    DeleteCsv(csvPath);
                    _logger.LogInformation($"LocationSyncJob - CSV Deleted");
                }

                _logger.LogInformation($"LocationSyncJob - Succeeded");
            }
        }

        private static dynamic CreateLocation(IReaderRow csv)
        {
            var latitude = csv.GetField<double?>("latitude");
            var longitude = csv.GetField<double?>("longitude");

            if (latitude == null || longitude == null)
            {
                return null;
            }

            var postcode = Location.SanitizePostcode(csv.GetField<string>("postcode"));

            return new { Postcode = postcode, Latitude = latitude, Longitude = longitude };
        }

        private static string GetTempPath()
        {
            return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        private async Task SyncLocations(string csvPath)
        {
            var batch = new List<dynamic>();
            var locationCount = 0;
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            await csv.ReadAsync();
            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                var location = CreateLocation(csv);
                if (location == null)
                {
                    continue;
                }

                batch.Add(location);
                locationCount++;

                QueueBatch(batch);
            }

            QueueBatch(batch, true);

            var batchJobCount = (int)Math.Ceiling((decimal)locationCount / BatchInterval);
            _logger.LogInformation($"LocationSyncJob - Queueing {locationCount} Locations ({batchJobCount} Jobs)");
        }

        private void QueueBatch(ICollection<dynamic> batch, bool force = false)
        {
            if (!force && batch.Count != BatchInterval)
            {
                return;
            }

            // Batch is serialized to pass by value.
            _jobClient.Enqueue<LocationBatchJob>(x => x.RunAsync(JsonConvert.SerializeObject(batch)));

            batch.Clear();
        }

        private async Task<string> RetrieveCsv(string ukPostcodeCsvUrl)
        {
            var zipPath = GetTempPath();
            var csvPath = GetTempPath();
            var net = new System.Net.WebClient();

            await net.DownloadFileTaskAsync(new Uri(ukPostcodeCsvUrl), zipPath);
            net.Dispose();

            _logger.LogInformation($"LocationSyncJob - ZIP Downloaded");

            try
            {
                ZipFile.ExtractToDirectory(zipPath, csvPath);
                _logger.LogInformation($"LocationSyncJob - CSV Extracted");
            }
            finally
            {
                File.Delete(zipPath);
                _logger.LogInformation($"LocationSyncJob - ZIP Deleted");
            }

            return Path.Combine(csvPath, UkPostcodeCsvFilename);
        }

        private void DeleteCsv(string csvPath)
        {
            File.Delete(csvPath);
        }
    }
}
