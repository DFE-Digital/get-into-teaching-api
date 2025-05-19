using FluentAssertions;
using FluentAssertions.Json;
using GetIntoTeachingApiTests.Helpers;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GetIntoTeachingApiTests.Contracts
{
    [Collection("Database")]
    public class TeacherTrainingAdviserTests : BaseTests
    {
        public TeacherTrainingAdviserTests(DatabaseFixture databaseFixture)
            : base(databaseFixture){
        }

        [Theory]
        [ContractTestInputs("./Contracts/Input/TeacherTrainingAdviser")]
        public async Task Contract(string scenario)
        {
            const string RequestUri = "/api/teacher_training_adviser/candidates";
            const string RequestMediaType = "application/json";
            StringContent content = new(ReadInput(scenario), Encoding.UTF8, RequestMediaType);
            await Setup();
            HttpResponseMessage response = await HttpClient.PostAsync(RequestUri, content);
            await AssertRequestMatchesSnapshot(scenario);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
