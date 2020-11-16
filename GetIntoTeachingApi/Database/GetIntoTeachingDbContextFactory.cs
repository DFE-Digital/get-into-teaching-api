using System;
using GetIntoTeachingApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GetIntoTeachingApi.Database
{
    public class GetIntoTeachingDbContextFactory : IDesignTimeDbContextFactory<GetIntoTeachingDbContext>
    {
        private readonly IEnv _env;

        public GetIntoTeachingDbContextFactory()
        {
            MockRequiredEnvironment();
            _env = new Env();
        }

        public GetIntoTeachingDbContextFactory(IEnv env)
        {
            _env = env;
        }

        public GetIntoTeachingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GetIntoTeachingDbContext>();
            var connectionString = DbConfiguration.DatabaseConnectionString(_env);

            DbConfiguration.ConfigPostgres(connectionString, optionsBuilder);

            return new GetIntoTeachingDbContext(optionsBuilder.Options);
        }

        private void MockRequiredEnvironment()
        {
            // We need to be able to generate a valid connection string
            // when creating migrations (even though the DB is never called).
            Environment.SetEnvironmentVariable("DATABASE_INSTANCE_NAME", "db-context-factory");
            Environment.SetEnvironmentVariable(
                "VCAP_SERVICES",
                "{\"postgres\":[{\"instance_name\":\"db-context-factory\", \"credentials\": " +
                "{\"host\":\"host\",\"name\":\"name\",\"username\":\"username\",\"password\":\"password\",\"port\":123}}]}");
        }
    }
}
