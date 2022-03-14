using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Utils;
using Microsoft.EntityFrameworkCore;
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
            $"{GenerateConnectionString(env, env.HangfireInstanceName)};SearchPath=hangfire";

        public static void ConfigPostgres(string connetionString, DbContextOptionsBuilder builder)
        {
            builder.UseNpgsql(connetionString, x => x.UseNetTopologySuite());
        }

        public void Migrate()
        {
            _dbContext.Database.Migrate();
        }

        // TODO: temp code to be removed as soon as its deployed/ran.
        public static void DropHangfireDatabase()
        {
            var env = new Env();

            if (!env.IsMasterInstance)
            {
                return;
            }

            var conn = new NpgsqlConnection(HangfireConnectionString(env));
            conn.Open();

            var sql = "SELECT COUNT(*) FROM hangfire.jobqueue";
            var cmd = new NpgsqlCommand(sql, conn);
            var count = Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.CurrentCulture);

            // Don't drop database and force deploy to abort if Hangfire has jobs in the queue.
            if (count != 0)
            {
                conn.Close();
                conn.Dispose();

                System.Environment.Exit(1);
            }

            var dropSql = "ALTER TABLE hangfire.state DROP CONSTRAINT state_jobid_fkey;" +
                "ALTER TABLE hangfire.jobparameter DROP CONSTRAINT jobparameter_jobid_fkey;" +
                "DROP TABLE hangfire.schema;" +
                "DROP TABLE hangfire.job;" +
                "DROP TABLE hangfire.state;" +
                "DROP TABLE hangfire.jobparameter;" +
                "DROP TABLE hangfire.jobqueue;" +
                "DROP TABLE hangfire.server;" +
                "DROP TABLE hangfire.list;" +
                "DROP TABLE hangfire.set;" +
                "DROP TABLE hangfire.lock;" +
                "DROP TABLE hangfire.counter;" +
                "DROP TABLE hangfire.hash;" +
                "DROP SCHEMA hangfire Cascade;";

            var dropCmd = new NpgsqlCommand(dropSql, conn);
            dropCmd.ExecuteNonQuery();

            conn.Close();
            conn.Dispose();
        }

        private static string GenerateConnectionString(IEnv env, string instanceName)
        {
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var vcap = JsonSerializer.Deserialize<VcapServices>(env.VcapServices, options);
            var postgres = vcap.Postgres.First(p => p.InstanceName == instanceName);

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = postgres.Credentials.Host,
                Database = postgres.Credentials.Name,
                Username = postgres.Credentials.Username,
                Password = postgres.Credentials.Password,
                Port = postgres.Credentials.Port,
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
            };

            return builder.ConnectionString;
        }

        internal class VcapServices
        {
            public IEnumerable<VcapPostgres> Postgres { get; set; }
        }

        internal class VcapPostgres
        {
            [JsonPropertyName("instance_name")]
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
