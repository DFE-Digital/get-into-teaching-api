using GetIntoTeachingApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GetIntoTeachingApi.Database
{
    public class GetIntoTeachingDbContextFactory : IDesignTimeDbContextFactory<GetIntoTeachingDbContext>
    {
        private readonly IEnv _env;

        public GetIntoTeachingDbContextFactory(IEnv env)
        {
            _env = env;
        }

        public GetIntoTeachingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GetIntoTeachingDbContext>();

            DbConfiguration.ConfigPostgres(_env, optionsBuilder);

            return new GetIntoTeachingDbContext(optionsBuilder.Options);
        }
    }
}
