using System;
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

            // Set environment for integration tests using the database.
            Environment.SetEnvironmentVariable("DATABASE_INSTANCE_NAME", TemplateDatabaseName);
            Environment.SetEnvironmentVariable("VCAP_SERVICES",
                $"{{\"postgres\": [{{\"instance_name\": \"{TemplateDatabaseName}\",\"credentials\": {{\"host\": \"localhost\"," +
                $"\"name\": \"{TemplateDatabaseName}\",\"username\": \"docker\",\"password\": \"docker\",\"port\": 5432}}}}]," +
                $"\"redis\": [{{\"credentials\": {{\"host\": \"0.0.0.0\",\"port\": 6379,\"password\": \"docker\",\"tls_enabled\": false}}}}]}}");

            var builder = new DbContextOptionsBuilder<GetIntoTeachingDbContext>();
            DbConfiguration.ConfigPostgres(ConnectionString, builder);
            _dbContext = new GetIntoTeachingDbContext(builder.Options);

            _dbContext.Database.Migrate();
            _dbContext.Database.CloseConnection();
        }
    }
}
