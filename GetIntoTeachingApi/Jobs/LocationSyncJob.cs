using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Utils;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Location = GetIntoTeachingApi.Models.Location;

namespace GetIntoTeachingApi.Jobs
{
    public class LocationSyncJob : BaseJob
    {
        public const string UkPostcodeCsvFilename = "ukpostcodes.csv";
        private const int BufferFlushInterval = 1000;
        private readonly GetIntoTeachingDbContext _dbContext;

        public LocationSyncJob(GetIntoTeachingDbContext dbContext)
        {
            _dbContext = dbContext;
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
            var buffer = new List<Location>();
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            await csv.ReadAsync();
            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                var location = CreateLocation(csv);
                if (location == null) continue;

                buffer.Add(location);

                await FlushBuffer(buffer);
            }

            await FlushBuffer(buffer, true);
        }

        private static Location CreateLocation(IReaderRow csv)
        {
            var latitude = csv.GetField<double?>("latitude");
            var longitude = csv.GetField<double?>("longitude");

            if (latitude == null || longitude == null) return null;

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: DbConfiguration.Wgs84Srid);
            var postcode = Location.SanitizePostcode(csv.GetField<string>("postcode"));
            var coordinate = geometryFactory.CreatePoint(new Coordinate((double) longitude, (double) latitude));

            return new Location() { Postcode = postcode, Coordinate = coordinate };
        }

        private async Task FlushBuffer(ICollection<Location> buffer, bool force = false)
        {
            if (!force && buffer.Count() != BufferFlushInterval)
                return;

            var bufferPostcodes = buffer.Select(l => l.Postcode);
            var existingPostcodes = _dbContext.Locations.Where(l => bufferPostcodes.Contains(l.Postcode)).Select(l => l.Postcode);
            var newLocations = buffer.Where(b => !existingPostcodes.Contains(b.Postcode));

            await _dbContext.Locations.AddRangeAsync(newLocations);
            await _dbContext.SaveChangesAsync();

            buffer.Clear();
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
