using System;
using System.IO;
using FluentAssertions;
using FluentAssertions.Json;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models.FindApply;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Contracts
{
    [Collection("Database")]
    public class ApplyCandidateApiTests : BaseTests
    {
        public ApplyCandidateApiTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            Environment.SetEnvironmentVariable($"APPLY_API_FEATURE", "on");
        }

        [Theory]
        [ContractTestInputs("./Contracts/Input/ApplyCandidateApi")]
        public async void Contract(string scenario)
        {
            await FlushState();

            var filename = $"{scenario.Replace(" ", "_")}.json";

            await SeedDatabase();

            var candidate = ConstructCandidate(ReadInput(filename));

            JobClient.Enqueue<FindApplyCandidateSyncJob>(c => c.Run(candidate));

            await WaitForAllJobsToComplete();

            var requestJson = RequestJson();
            var request = SortEntities(JArray.Parse(requestJson));
            var outputFile = OutputFilePath(filename);

            await WriteInitialOutputFile(outputFile, request);

            var snapshot = SortEntities(JArray.Parse(File.ReadAllText(outputFile)));

            request.Should().HaveCount(snapshot.Count);
            request.Should().BeEquivalentTo(snapshot);
        }

        private static Candidate ConstructCandidate(string json)
        {
            return JsonConvert.DeserializeObject<Candidate>(json);
        }
    }
}
