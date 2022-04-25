using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GetIntoTeachingApi.AppStart;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetIntoTeachingApiTests.Contracts
{
	public class BaseTests : DatabaseTests
	{
        private static readonly int maxWaitDurationInSeconds = 60;
        private static readonly int waitInterval = 200;
        private static readonly int verifyChallenges = 3;
        private readonly ContractTestGitWebApplicationFactory<Startup> _factory;
        protected readonly HttpClient _httpClient;
        public IBackgroundJobClient JobClient
        {
            get
            {
                return (IBackgroundJobClient) _factory.Services.GetService(typeof(IBackgroundJobClient));
            }
        }
        public string ContractName
        {
            get
            {
                return GetType().Name.Replace("Tests", string.Empty);
            }
        }

        public BaseTests(DatabaseFixture databaseFixture)
            : base(databaseFixture)
        {
            var apiKey = "admin-secret";
            Environment.SetEnvironmentVariable($"ADMIN_API_KEY", apiKey);

            _factory = new ContractTestGitWebApplicationFactory<Startup>(GetContractTestState());
            _httpClient = _factory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);
        }

        protected string ReadInput(string filename)
        {
            var file = $"./Contracts/Input/{ContractName}/{filename}";
            return File.ReadAllText(file);
        }

        protected string OutputFilePath(string filename)
        {
            var relativePath = $"../../../Contracts/Output/{ContractName}/{filename}";
            return Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        }

        protected static JArray SortEntities(JArray entities)
        {
            entities.ForEach(e => e["Attributes"] = new JArray(e["Attributes"].OrderBy(a => a["Key"])));

            return new JArray(entities.OrderBy(e => e["LogicalName"]));
        }

        protected static async Task WriteInitialOutputFile(string outputFile, JArray request)
        {
            if (!File.Exists(outputFile))
            {
                await File.WriteAllTextAsync(outputFile, JsonConvert.SerializeObject(request, Formatting.Indented));
            }
        }

        protected static ContractTestState GetContractTestState()
        {
            var json = File.ReadAllText("./Contracts/Data/state.json");
            return JsonConvert.DeserializeObject<ContractTestState>(json);
        }

        protected string RequestJson()
        {
            var trackedEntities = _factory.ContractOrganizationServiceAdapter.TrackedEntities;
            return JsonConvert.SerializeObject(trackedEntities, Formatting.Indented);
        }

        protected async Task SeedDatabase()
        {
            await SeedPickListItems();
            await SeedLookupItems();
            await SeedPrivacyPolicy();
        }

        private async Task SeedPickListItems()
        {
            var files = Directory.EnumerateFiles("./Contracts/Data/pick_list_items",
                "*.json", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                var json = File.ReadAllText(file);
                var items = JsonConvert.DeserializeObject<IEnumerable<PickListItem>>(json);
                await DbContext.AddRangeAsync(items);
            }

            await DbContext.SaveChangesAsync();
        }

        private async Task SeedLookupItems()
        {
            var files = Directory.EnumerateFiles("./Contracts/Data/lookup_items",
                "*.json", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                var json = File.ReadAllText(file);
                var items = JsonConvert.DeserializeObject<IEnumerable<LookupItem>>(json);
                await DbContext.AddRangeAsync(items);
            }

            await DbContext.SaveChangesAsync();
        }

        protected async Task FlushState()
        {
            ClearJobQueue();
            await WaitForAllJobsToComplete();
            _factory.ContractOrganizationServiceAdapter.TrackedEntities.Clear();
        }

        private void ClearJobQueue()
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var queues = monitoringApi.Queues();

            foreach (var queue in queues)
            {
                monitoringApi.EnqueuedJobs(queue.Name, 0, int.MaxValue).ForEach(t => JobClient.Delete(t.Key));
            }
        }

        protected static async Task WaitForAllJobsToComplete()
        {
            var startWaitTime = DateTime.UtcNow;

            while (!await VerifyJobsAreComplete())
            {
                await Task.Delay(waitInterval);

                (DateTime.UtcNow - startWaitTime).TotalSeconds.Should().BeLessOrEqualTo(maxWaitDurationInSeconds);
            }
        }

        /*
         * As we have multiple jobs - and jobs that spawn other jobs - we need
         * to be certain all jobs have been scheduled and are complete before continuing.
         * 
         * If there are pending/enqueued jobs we can immediately return false (and the calling code
         * is expected to wait a short period before checking again).
         * 
         * If there are no pending/enqueued jobs we need to perform repeat, delayed checks
         * as one may be in the process of being queued (asynchronously). If after multiple checks
         * there are still no pending/enqueued jobs it should be safe to assume they're all finished.
         */
        private async static Task<bool> VerifyJobsAreComplete()
        {
            if (JobsPending())
            {
                return false;
            }

            for (var attmept = 0; attmept < verifyChallenges; attmept++)
            {
                await Task.Delay(waitInterval);

                if (JobsPending())
                {
                    return false;
                }
            }

            return true;
        }

        private static bool JobsPending()
        {
            var jobStats = JobStorage.Current.GetMonitoringApi().GetStatistics();

            return jobStats.Enqueued + jobStats.Processing != 0;
        }

        private async Task SeedPrivacyPolicy()
        {
            var json = File.ReadAllText("./Contracts/Data/privacy_policy.json");
            var policy = JsonConvert.DeserializeObject<PrivacyPolicy>(json);
            await DbContext.AddAsync(policy);
            await DbContext.SaveChangesAsync();
        }
    }
}

