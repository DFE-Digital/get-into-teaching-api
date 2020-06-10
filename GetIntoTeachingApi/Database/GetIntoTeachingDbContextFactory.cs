using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GetIntoTeachingApi.Database
{
    public class GetIntoTeachingDbContextFactory : IDesignTimeDbContextFactory<GetIntoTeachingDbContext>
    {
        public GetIntoTeachingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GetIntoTeachingDbContext>();

            DbConfiguration.ConfigPostgres(optionsBuilder);

            return new GetIntoTeachingDbContext(optionsBuilder.Options);
        }
    }
}
