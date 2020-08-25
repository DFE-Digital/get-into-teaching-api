using GetIntoTeachingApi.Services;
using GetIntoTeachingApiContractTests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GetIntoTeachingApiContractTests
{
    public class TestStartup : GetIntoTeachingApi.Startup
    {
        public TestStartup(IConfiguration configuration)
            :base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment hostEnv)
        {
            base.Configure(app, hostEnv);
            
            using var serviceScope = app.ApplicationServices.CreateScope();
            
            var store = serviceScope.ServiceProvider.GetService<IStore>();
            var crmService = serviceScope.ServiceProvider.GetService<ICrmService>();
            
            AsyncHelper.RunSync(() => store.SyncAsync(crmService));
        }

    }
}