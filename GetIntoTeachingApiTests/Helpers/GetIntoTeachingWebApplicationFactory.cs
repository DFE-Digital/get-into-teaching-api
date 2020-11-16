using GetIntoTeachingApi;
using GetIntoTeachingApi.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GetIntoTeachingApiTests.Helpers
{
    public class GetIntoTeachingWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private readonly GetIntoTeachingDbContext _dbContext;

        public GetIntoTeachingWebApplicationFactory(GetIntoTeachingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
                services.AddDbContext<GetIntoTeachingDbContext>(b => DbConfiguration.ConfigPostgres(connectionString, b));
            });
        }
    }
}
