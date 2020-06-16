using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;

namespace GetIntoTeachingApi.Database
{
    public class DbConfiguration
    {
        public const int Wgs84Srid = 4326;
        public const int UkSrid = 27700;
        private readonly GetIntoTeachingDbContext _dbContext;

        public DbConfiguration(GetIntoTeachingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public static string DatabaseConnectionString(IEnv env) => 
            GenerateConnectionString(env, env.DatabaseInstanceName);

        public static string HangfireConnectionString(IEnv env) => 
            GenerateConnectionString(env, env.HangfireInstanceName);

        public static void ConfigPostgres(IEnv env, DbContextOptionsBuilder builder)
        {
            builder.UseNpgsql(DbConfiguration.DatabaseConnectionString(env), x => x.UseNetTopologySuite());
        }

        public static void ConfigSqLite(DbContextOptionsBuilder builder, SqliteConnection keepAliveConnection)
        {
            keepAliveConnection.Open();
            builder.UseSqlite(keepAliveConnection, x => x.UseNetTopologySuite());
        }

        public void Configure()
        {
            var migrationsAreSupported = _dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite";

            if (migrationsAreSupported)
                _dbContext.Database.Migrate();
            else 
                _dbContext.Database.EnsureCreated();
        }

        private static string GenerateConnectionString(IEnv env, string instanceName)
        {
            var vcap = JsonConvert.DeserializeObject<VcapServices>(env.VcapServices);
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
