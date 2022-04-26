using System;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models.FindApply;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Contracts
{
    [Collection("Database")]
    public class ApplyCandidateApiTests : BaseTests
    {
        public ApplyCandidateApiTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            Environment.SetEnvironmentVariable($"APPLY_API_FEATURE", "on");
            Environment.SetEnvironmentVariable($"APPLY_API_V1_2_FEATURE", "on");
        }

        [Theory]
        [ContractTestInputs("./Contracts/Input/ApplyCandidateApi")]
        public async void Contract(string scenario)
        {
            await Setup();

            var candidate = ConstructCandidate(ReadInput(scenario));
            JobClient.Enqueue<FindApplyCandidateSyncJob>(c => c.Run(candidate));

            await AssertRequestMatchesSnapshot(scenario);
        }

        private static Candidate ConstructCandidate(string json)
        {
            return JsonConvert.DeserializeObject<Candidate>(json);
        }
    }
}
