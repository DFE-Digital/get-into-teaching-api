using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using FluentAssertions.Json;
using GetIntoTeachingApiTests.Helpers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Contracts
{
    [Collection("Database")]
    public class TeacherTrainingAdviserTests : BaseTests
    {
        public TeacherTrainingAdviserTests(DatabaseFixture databaseFixture)
            : base(databaseFixture)
        {
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

            var request = SortEntities(JArray.Parse(RequestJson()));
            var outputFile = OutputFilePath(filename);

            await WriteInitialOutputFile(outputFile, request);

            var snapshot = SortEntities(JArray.Parse(File.ReadAllText(outputFile)));

            request.Should().HaveCount(snapshot.Count);
            request.Should().BeEquivalentTo(snapshot);
        }

        private StringContent ConstructBody(string filename)
        {
            return new StringContent(ReadInput(filename), Encoding.UTF8, "application/json");
        }
    }
}
