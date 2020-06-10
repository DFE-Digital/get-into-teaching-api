using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Newtonsoft.Json;

namespace GetIntoTeachingApi.Jobs
{
    public class LocationSyncJob : BaseJob
    {
        public const string UkPostcodeCsvFilename = "ukpostcodes.csv";
        private const int BatchInterval = 100;
        private readonly IBackgroundJobClient _jobClient;

        public LocationSyncJob(IBackgroundJobClient jobClient)
        {
            _jobClient = jobClient;
        }

        public async Task RunAsync(string ukPostcodeCsvUrl)
        {
            var csvPath = await RetrieveCsv(ukPostcodeCsvUrl);

            try
            {
                await SyncLocations(csvPath);
            }
            finally
            {
                DeleteCsv(csvPath);
            }
        }

        private async Task SyncLocations(string csvPath)
        {
            var batch = new List<dynamic>();
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            await csv.ReadAsync();
            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                var location = CreateLocation(csv);
                if (location == null) continue;

                batch.Add(location);

                QueueBatch(batch);
            }

            QueueBatch(batch, true);
        }

        private static dynamic CreateLocation(IReaderRow csv)
        {
            var latitude = csv.GetField<double?>("latitude");
            var longitude = csv.GetField<double?>("longitude");

            if (latitude == null || longitude == null) return null;

            var postcode = Location.SanitizePostcode(csv.GetField<string>("postcode"));

            return new { Postcode = postcode, Latitude = latitude, Longitude = longitude };
        }

        private void QueueBatch(ICollection<dynamic> batch, bool force = false)
        {
            if (!force && batch.Count() != BatchInterval)
                return;

            // Batch is serialized to pass by value.
            _jobClient.Enqueue<LocationBatchJob>(x => x.RunAsync(JsonConvert.SerializeObject(batch)));

            batch.Clear();
        }

        private static async Task<string> RetrieveCsv(string ukPostcodeCsvUrl)
        {
            if (Env.IsDevelopment)
                return "./Fixtures/ukpostcodes.dev.csv";

            var zipPath = GetTempPath();
            var csvPath = GetTempPath();
            var net = new System.Net.WebClient();

            await net.DownloadFileTaskAsync(new Uri(ukPostcodeCsvUrl), zipPath);

            try
            {
                ZipFile.ExtractToDirectory(zipPath, csvPath);
            }
            finally
            {
                File.Delete(zipPath);
            }
            
            return Path.Combine(csvPath, UkPostcodeCsvFilename);
        }

        private static void DeleteCsv(string csvPath)
        {
            if (!Env.IsDevelopment)
            {
                File.Delete(csvPath);
            }
        }

        private static string GetTempPath()
        {
            return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }
    }
}
