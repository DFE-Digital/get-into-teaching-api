using System;
using System.IO;
using System.Reflection;
using GetIntoTeachingApi.Adapters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApiContractTests.Servers
{
    public class ServerUnderTest : TestServer
    {
        public static TestOrganizationServiceAdapter CrmServiceAdapter { get; protected set; }

        protected ServerUnderTest(IWebHostBuilder builder)
            : base(builder)
        {
            
        }
    }
    
    public class ServerUnderTest<TStartup> : ServerUnderTest
    {
        private static readonly Assembly STARTUP_ASSEMBLY = typeof(TStartup).GetTypeInfo().Assembly;
        
        public ServerUnderTest(string relativeTargetProjectParentDir)
            : base(Setup(relativeTargetProjectParentDir))
        {
            
        }
        
        private static IWebHostBuilder Setup(string relativeTargetProjectParentDir)
        {
            var contentRoot = GetProjectPath(relativeTargetProjectParentDir, STARTUP_ASSEMBLY);

            var configurationBuilder = new ConfigurationBuilder()
                    .SetBasePath(contentRoot)
                    .AddJsonFile("appsettings.json")
                ;

            var webHostBuilder = new WebHostBuilder()
                    .UseContentRoot(contentRoot)
                    .ConfigureServices(InitializeServices) 
                    .ConfigureTestServices(services =>
                    {
                        services.AddTransient<IOrganizationServiceAdapter>(sp =>
                        {
                            CrmServiceAdapter = new TestOrganizationServiceAdapter(sp.GetService<IOrganizationService>(), contentRoot);
                            return CrmServiceAdapter;
                        });
                    })
                    .UseConfiguration(configurationBuilder.Build())
                    .UseEnvironment("Test")
                    .UseStartup(typeof(TestStartup))
                ;

            // Create instance of test server
            return webHostBuilder;
        }

        private static void InitializeServices(IServiceCollection services)
        {
            var manager = new ApplicationPartManager
            {
                ApplicationParts =
                {
                    new AssemblyPart(STARTUP_ASSEMBLY)
                },
                FeatureProviders =
                {
                    new ControllerFeatureProvider(),
                    new ViewComponentFeatureProvider()
                }
            };

            services.AddSingleton(manager);
        }

        private static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
        {
            var projectName = startupAssembly.GetName().Name;
            var applicationBasePath = AppContext.BaseDirectory;
            var directoryInfo = new DirectoryInfo(applicationBasePath);

            do
            {
                directoryInfo = directoryInfo.Parent;

                if (directoryInfo == null) continue;
                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));

                if (!projectDirectoryInfo.Exists) continue;
                if (new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName!, $"{projectName}.csproj")).Exists)
                    return Path.Combine(projectDirectoryInfo.FullName, projectName);
            }
            while (directoryInfo?.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }
    }
}