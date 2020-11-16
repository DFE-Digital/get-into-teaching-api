using System;
using GetIntoTeachingApi.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GetIntoTeachingApiTests.Helpers
{
    public abstract class DatabaseTests : IDisposable
    {
        public GetIntoTeachingDbContext DbContext { get; set; }

        protected DatabaseTests(DatabaseFixture databaseFixture)
        {
            var id = Guid.NewGuid().ToString().Replace("-", "");
            var databaseName = $"gis_test_{id}";

            CloneTemplateDatabase(databaseFixture, databaseName);
            SetupDbContext(databaseName);
        }

        public void Dispose()
        {
            try
            {
                DbContext.Database.EnsureDeleted();
            } catch(ObjectDisposedException)
            {
                // StoreTests.CheckStatusAsync_WhenUnhealthy_ReturnsError
                // disposes of the connection prematurely as part of the test.
            }
        }

        private void SetupDbContext(string databaseName)
        {
            var connectionString = $"Host=localhost;Database={databaseName};Username=docker;Password=docker";
            var builder = new DbContextOptionsBuilder<GetIntoTeachingDbContext>();

            DbConfiguration.ConfigPostgres(connectionString, builder);

            DbContext = new GetIntoTeachingDbContext(builder.Options);
        }

        private static void CloneTemplateDatabase(DatabaseFixture databaseFixture, string databaseName)
        {
            using var templateConnection = new NpgsqlConnection(databaseFixture.ConnectionString);
            templateConnection.Open();

            using var command = new NpgsqlCommand($"CREATE DATABASE {databaseName} WITH TEMPLATE {databaseFixture.TemplateDatabaseName};", templateConnection);
            command.ExecuteNonQuery();
        }
    }
}
