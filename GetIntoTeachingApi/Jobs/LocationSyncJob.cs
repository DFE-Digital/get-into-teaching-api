using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Prometheus;

namespace GetIntoTeachingApi.Jobs
{
    public class LocationSyncJob : BaseJob
    {
        public const string UkPostcodeCsvFilename = "ukpostcodes.csv";
        public static readonly string FreeMapToolsUrl = "https://www.freemaptools.com/download/full-uk-postcodes/ukpostcodes.zip";
        private const int BatchInterval = 300;
        private readonly ILogger<LocationSyncJob> _logger;
        private readonly IMetricService _metrics;
        private readonly GetIntoTeachingDbContext _dbContext;

        public LocationSyncJob(
            IEnv env,
            IRedisService redis,
            GetIntoTeachingDbContext dbContext,
            ILogger<LocationSyncJob> logger,
            IMetricService metrics)
            : base(env, redis)
        {
            _logger = logger;
            _dbContext = dbContext;
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

        private static Location CreateLocation(IReaderRow csv)
        {
            var latitude = csv.GetField<double?>(2);
            var longitude = csv.GetField<double?>(3);

            if (latitude == null || longitude == null)
            {
                return null;
            }

            var postcode = csv.GetField<string>(1);

            return new Location(postcode, (double)latitude, (double)longitude, Location.SourceType.CSV);
        }

        private static bool IsHeaderRow(IReaderRow csv)
        {
            return csv.GetField<string>(0) == "id" &&
                csv.GetField<string>(1) == "postcode" &&
                csv.GetField<string>(2) == "latitude" &&
                csv.GetField<string>(3) == "longitude";
        }

        private static string GetTempPath()
        {
            return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        private static NpgsqlParameter[] DbParameters(Location location, int index)
        {
            return new[]
            {
                new NpgsqlParameter($"postcode{index}", location.Postcode),
                new NpgsqlParameter($"coordinate{index}", location.Coordinate),
                new NpgsqlParameter($"source{index}", (int)Location.SourceType.CSV),
            };
        }

        private static void DeleteCsv(string csvPath)
        {
            File.Delete(csvPath);
        }

        private async Task SyncLocations(string csvPath)
        {
            var batch = new List<Location>();
            var locationCount = 0;
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            while (await csv.ReadAsync())
            {
                if (IsHeaderRow(csv))
                {
                    continue;
                }

                var location = CreateLocation(csv);
                if (location == null)
                {
                    continue;
                }

                batch.Add(location);
                locationCount++;

                await ProcessBatch(batch);
            }

            await ProcessBatch(batch, true);

            var batchCount = (int)Math.Ceiling((decimal)locationCount / BatchInterval);
            _logger.LogInformation("LocationSyncJob - Processed {LocationCount} Locations ({BatchCount} Batches)", locationCount, batchCount);
        }

        private async Task ProcessBatch(ICollection<Location> batch, bool force = false)
        {
            if (!force && batch.Count != BatchInterval)
            {
                return;
            }

            var parameters = batch.SelectMany((location, index) => DbParameters(location, index));
            var values = string.Join(", ", batch.Select((_, index) => $"(@postcode{index}, @coordinate{index}, @source{index})"));

            await _dbContext.Database.ExecuteSqlRawAsync(
                $"INSERT INTO public.\"Locations\" (\"Postcode\", \"Coordinate\", \"Source\") VALUES {values} ON CONFLICT DO NOTHING", parameters);

            batch.Clear();
        }

        private async Task<string> RetrieveCsv(string ukPostcodeCsvUrl)
        {
            var zipPath = GetTempPath();
            var csvPath = GetTempPath();
            using var client = new HttpClient();
            var response = await client.GetAsync(new Uri(ukPostcodeCsvUrl));

            using (var stream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await response.Content.CopyToAsync(stream);
            }

            _logger.LogInformation("LocationSyncJob - ZIP Downloaded");

            try
            {
                var checker = new ZipFileChecker();
                checker.AssureNoBombs(zipPath);
                ZipFile.ExtractToDirectory(zipPath, csvPath);
                _logger.LogInformation("LocationSyncJob - CSV Extracted");
            }
            finally
            {
                File.Delete(zipPath);
                _logger.LogInformation("LocationSyncJob - ZIP Deleted");
            }

            return Path.Combine(csvPath, UkPostcodeCsvFilename);
        }
    }
}
