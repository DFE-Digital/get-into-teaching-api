using System;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GetIntoTeachingApiTests.Helpers
{
    public abstract class DatabaseTests : IDisposable
    {
        private readonly SqliteConnection _keepAliveConnection;
        protected readonly GetIntoTeachingDbContext DbContext;

        protected DatabaseTests()
        {
            _keepAliveConnection = new SqliteConnection("DataSource=:memory:");

            var envMock = new Mock<IEnv>();
            var builder = new DbContextOptionsBuilder<GetIntoTeachingDbContext>();
            DbConfiguration.ConfigSqLite(builder, _keepAliveConnection);

            DbContext = new GetIntoTeachingDbContext(builder.Options);
            var dbConfiguration = new DbConfiguration(DbContext);
            dbConfiguration.Configure(envMock.Object);
        }

        public void Dispose() => _keepAliveConnection.Dispose();
    }
}
