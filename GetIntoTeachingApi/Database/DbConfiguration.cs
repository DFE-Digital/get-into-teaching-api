﻿using System;
using System.Collections.Generic;
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

        private static string GenerateConnectionString(IEnv env, string instanceName)
        {
            if (!string.IsNullOrEmpty(env.PgConnectionString))
            {
                return env.PgConnectionString;
            }

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
            };

            return builder.ConnectionString;
        }

        internal sealed class VcapServices
        {
            public IEnumerable<VcapPostgres> Postgres { get; set; }
        }

        internal sealed class VcapPostgres
        {
            [JsonPropertyName("instance_name")]
            public string InstanceName { get; set; }
            public VcapCredentials Credentials { get; set; }
        }

        internal sealed class VcapCredentials
        {
            public string Host { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int Port { get; set; }
        }
    }
}
