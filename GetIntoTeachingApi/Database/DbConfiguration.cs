using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using GetIntoTeachingApi.Utils;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Npgsql;
using Location = GetIntoTeachingApi.Models.Location;

namespace GetIntoTeachingApi.Database
{
    public class DbConfiguration
    {
        public const int Wgs84Srid = 4326;
        public const int UkSrid = 27700;
        private const int BufferFlushInterval = 1000;
        private readonly GetIntoTeachingDbContext _dbContext;

        public DbConfiguration(GetIntoTeachingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public static string DatabaseConnectionString() => GenerateConnectionString(Environment.GetEnvironmentVariable("DATABASE_INSTANCE_NAME"));

        public static string HangfireConnectionString() => GenerateConnectionString(Environment.GetEnvironmentVariable("HANGFIRE_INSTANCE_NAME"));

        public void Configure()
        {
            var migrationsAreSupported = _dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite";

            if (migrationsAreSupported)
                _dbContext.Database.Migrate();
            else 
                _dbContext.Database.EnsureCreated();

            SeedLocations();
        }

        private void SeedLocations()
        {
            if (_dbContext.Locations.Any()) return;

            var buffer = new List<Location>();
            using var reader = new StreamReader(LocationsFixturePath());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                var location = CreateLocation(csv);

                if (location == null) continue;

                buffer.Add(location);

                FlushBuffer(buffer, _dbContext.Locations);
            }

            FlushBuffer(buffer, _dbContext.Locations, true);
        }

        private static Location CreateLocation(IReaderRow csv)
        {
            var latitude = csv.GetField<double?>("latitude");
            var longitude = csv.GetField<double?>("longitude");

            if (latitude == null || longitude == null) return null;

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: Wgs84Srid);

            return new Location()
            {
                Postcode = Location.SanitizePostcode(csv.GetField<string>("postcode")),
                Coordinate = geometryFactory.CreatePoint(new Coordinate((double)longitude, (double)latitude))
            };
        }

        private void FlushBuffer<T>(ICollection<T> buffer, DbSet<T> dbSet, bool force = false) where T : class
        {
            if (!force && buffer.Count() != BufferFlushInterval)
                return;

            dbSet.AddRange(buffer);
            _dbContext.SaveChanges();
            buffer.Clear();
        }

        private static string LocationsFixturePath() =>
            Env.IsDevelopment ? "./Fixtures/ukpostcodes.dev.csv" : "./Fixtures/ukpostcodes.csv";

        private static string GenerateConnectionString(string instanceName)
        {
            var vcap = JsonConvert.DeserializeObject<VcapServices>(new Env().VcapServices);
            var postgres = vcap.Postgres.First(p => p.InstanceName == instanceName);

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = postgres.Credentials.Host,
                Database = postgres.Credentials.Name,
                Username = postgres.Credentials.Username,
                Password = postgres.Credentials.Password,
                Port = postgres.Credentials.Port,
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return builder.ConnectionString;
        }

        internal class VcapServices
        {
            public IEnumerable<VcapPostgres> Postgres { get; set; }
        }

        internal class VcapPostgres
        {
            [JsonProperty("instance_name")]
            public string InstanceName { get; set; }
            public VcapCredentials Credentials { get; set; }
        }

        internal class VcapCredentials
        {
            public string Host { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int Port { get; set; }
        }
    }
}
