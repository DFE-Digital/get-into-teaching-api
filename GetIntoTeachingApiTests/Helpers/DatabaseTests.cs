using System;
using GetIntoTeachingApi.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GetIntoTeachingApiTests.Helpers
{
    public abstract class DatabaseTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        protected readonly GetIntoTeachingDbContext DbContext;

        protected DatabaseTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<GetIntoTeachingDbContext>()
                .UseSqlite(_connection, x => x.UseNetTopologySuite()).Options;
            DbContext = new GetIntoTeachingDbContext(options);
            new DbConfiguration(DbContext).Configure();
        }

        public void Dispose() => _connection.Dispose();
    }
}
