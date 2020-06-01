using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;

namespace GetIntoTeachingApi.Database
{
    public class DbConfiguration
    {
        private const int BufferFlushInterval = 1000;
        private readonly GetIntoTeachingDbContext _dbContext;

        public DbConfiguration(GetIntoTeachingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public static string DatabaseConnectionString() => GenerateConnectionString("get-into-teaching-api-dev-pg-svc-2");

        public static string HangfireConnectionString() => GenerateConnectionString("get-into-teaching-api-dev-pg-svc");

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

                if (location.IsNonGeographic()) continue;

                buffer.Add(location);

                FlushBuffer(buffer, _dbContext.Locations);
            }

            FlushBuffer(buffer, _dbContext.Locations, true);
        }

        private static Location CreateLocation(CsvReader csv)
        {
            return new Location()
            {
                Postcode = LocationService.Sanitize(csv.GetField<string>("postcode")),
                Latitude = csv.GetField<double?>("latitude"),
                Longitude = csv.GetField<double?>("longitude")
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
            var vcap = JsonConvert.DeserializeObject<VcapServices>(Environment.GetEnvironmentVariable("VCAP_SERVICES"));
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
