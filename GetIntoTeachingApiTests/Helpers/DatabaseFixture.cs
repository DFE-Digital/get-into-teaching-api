using GetIntoTeachingApi.Database;
using Microsoft.EntityFrameworkCore;

namespace GetIntoTeachingApiTests.Helpers
{
    public class DatabaseFixture
    {
        public string TemplateDatabaseName { get; }
        public string ConnectionString { get; }
        private readonly GetIntoTeachingDbContext _dbContext;

        public DatabaseFixture()
        {
            TemplateDatabaseName = $"gis_test";
            ConnectionString = $"Host=localhost;Database={TemplateDatabaseName};Username=docker;Password=docker";

            var builder = new DbContextOptionsBuilder<GetIntoTeachingDbContext>();
            DbConfiguration.ConfigPostgres(ConnectionString, builder);
            _dbContext = new GetIntoTeachingDbContext(builder.Options);

            _dbContext.Database.Migrate();
            _dbContext.Database.CloseConnection();
        }
    }
}
