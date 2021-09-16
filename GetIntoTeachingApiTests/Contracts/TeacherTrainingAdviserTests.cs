using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Json;
using GetIntoTeachingApi.AppStart;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Contracts
{
    [Collection("Database")]
    public class TeacherTrainingAdviserTests : DatabaseTests
    {
        private static readonly int maxWaitDurationInSeconds = 60;
        private static readonly int waitInterval = 200;
        private static readonly int VerifyChallenges = 3;
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

            var request = SortEntities(JArray.Parse(requestJson));
            var snapshot = SortEntities(JArray.Parse(File.ReadAllText(outputFile)));

            request.Should().HaveCount(snapshot.Count());
            request.Should().BeEquivalentTo(snapshot);
        }

        private static JArray SortEntities(JArray entities)
        {
            entities.ForEach(e => e["Attributes"] = new JArray(e["Attributes"].OrderBy(a => a["Key"])));

            return new JArray(entities.OrderBy(e => e["LogicalName"]));
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
            var trackedEntities = _factory.ContractOrganizationServiceAdapter.TrackedEntities;
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

            for (var attmept = 0; attmept < VerifyChallenges; attmept++)
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
