using System;
using GetIntoTeachingApi.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GetIntoTeachingApiTests.Helpers
{
    public abstract class DatabaseTests : IDisposable
    {
        public GetIntoTeachingDbContext DbContext { get; }

        protected DatabaseTests(DatabaseFixture databaseFixture)
        {
            var id = Guid.NewGuid().ToString().Replace("-", "");
            var databaseName = $"gis_test_{id}";
            var connectionString = $"Host=localhost;Database={databaseName};Username=docker;Password=docker";

            using var templateConnection = new NpgsqlConnection(databaseFixture.ConnectionString);
            templateConnection.Open();

            using var command = new NpgsqlCommand($"CREATE DATABASE {databaseName} WITH TEMPLATE {databaseFixture.TemplateDatabaseName};", templateConnection);
            command.ExecuteNonQuery();
                
            var builder = new DbContextOptionsBuilder<GetIntoTeachingDbContext>();
            DbConfiguration.ConfigPostgres(connectionString, builder);
            DbContext = new GetIntoTeachingDbContext(builder.Options);
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
    }
}
