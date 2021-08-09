using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GetIntoTeachingApi.AppStart;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Contracts
{
    [Collection("Database")]
    public class TeacherTrainingAdviserTests : DatabaseTests
    {
        private static readonly int maxWaitDuration = 600000;
        private static readonly int waitInterval = 200;
        private readonly HttpClient _httpClient;
        private readonly ContractTestGitWebApplicationFactory<Startup> _factory;

        public TeacherTrainingAdviserTests(DatabaseFixture databaseFixture)
            : base(databaseFixture)
        {
            var apiKey = "admin-secret";
            Environment.SetEnvironmentVariable($"ADMIN_API_KEY", apiKey);

            _factory = new ContractTestGitWebApplicationFactory<Startup>(GetContractTestState());
            _httpClient = _factory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);
        }

        [Theory]
        [ContractTestInputs("./Contracts/Input/TeacherTrainingAdviser")]
        public async void Contract(string scenario)
        {
            await FlushState();

            var filename = $"{scenario.Replace(" ", "_")}.json";

            await SeedDatabase();

            var response = await _httpClient.PostAsync(
                "/api/teacher_training_adviser/candidates", ConstructBody(filename));

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await WaitForAllJobsToComplete();

            var requestJson = RequestJson();
            var outputFile = OutputFilePath(filename);

            await WriteInitialOutputFile(outputFile, requestJson);

            requestJson.Should().BeEquivalentTo(File.ReadAllText(outputFile));
        }

        private static StringContent ConstructBody(string filename)
        {
            var file = $"./Contracts/Input/TeacherTrainingAdviser/{filename}";
            var json = File.ReadAllText(file);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static async Task WriteInitialOutputFile(string outputFile, string requestJson)
        {
            if (!File.Exists(outputFile))
            {
                await File.WriteAllTextAsync(outputFile, requestJson);
            }
        }

        private static string OutputFilePath(string filename)
        {
            var relativePath = $"../../../Contracts/Output/TeacherTrainingAdviser/{filename}";
            return Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        }

        private string RequestJson()
        {
            var trackedEntities = _factory.ContractOrganizationServiceAdapter.TrackedEntities.OrderBy(e => e.LogicalName);
            return JsonConvert.SerializeObject(trackedEntities, Formatting.Indented);
        }

        private async Task SeedDatabase()
        {
            await SeedPickListItems();
            await SeedLookupItems();
            await SeedPrivacyPolicy();
        }

        private ContractTestState GetContractTestState()
        {
            var json = File.ReadAllText("./Contracts/Data/state.json");
            return JsonConvert.DeserializeObject<ContractTestState>(json);
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
        
        private async Task FlushState()
        {
            ClearJobQueue();
            await WaitForAllJobsToComplete();
            _factory.ContractOrganizationServiceAdapter.TrackedEntities.Clear();
        }

        private static void ClearJobQueue()
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var queues = monitoringApi.Queues();

            foreach (var queue in queues)
            {
                monitoringApi.EnqueuedJobs(queue.Name, 0, int.MaxValue).ForEach(t => BackgroundJob.Delete(t.Key));
            }
        }

        private static async Task WaitForAllJobsToComplete()
        {
            var allJobsComplete = false;
            var timeWaited = 0;

            while (!allJobsComplete)
            {
                await Task.Delay(waitInterval);

                var jobStats = JobStorage.Current.GetMonitoringApi().GetStatistics();
                allJobsComplete = jobStats.Enqueued + jobStats.Processing == 0;

                timeWaited += waitInterval;
                timeWaited.Should().BeLessOrEqualTo(maxWaitDuration);
            }
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
