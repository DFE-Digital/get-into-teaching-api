using System;
using GetIntoTeachingApi.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GetIntoTeachingApiTests.Helpers
{
    public abstract class DatabaseTests : IDisposable
    {
        private readonly SqliteConnection _keepAliveConnection;
        protected readonly GetIntoTeachingDbContext DbContext;

        protected DatabaseTests()
        {
            _keepAliveConnection = new SqliteConnection("DataSource=:memory:");

            var builder = new DbContextOptionsBuilder<GetIntoTeachingDbContext>();
            DbConfiguration.ConfigSqLite(builder, _keepAliveConnection);

            DbContext = new GetIntoTeachingDbContext(builder.Options);
            new DbConfiguration(DbContext).Configure();
        }

        public void Dispose() => _keepAliveConnection.Dispose();
    }
}
