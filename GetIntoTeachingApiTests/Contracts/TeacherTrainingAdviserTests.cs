using System;
using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using FluentAssertions.Json;
using GetIntoTeachingApiTests.Helpers;
using System.Threading.Tasks;
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
        public async Task Contract(string scenario)
        {
            await Setup();

            var response = await HttpClient.PostAsync(
                "/api/teacher_training_adviser/candidates", ConstructBody(scenario));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await AssertRequestMatchesSnapshot(scenario);
        }

        private StringContent ConstructBody(string scenario)
        {
            return new StringContent(ReadInput(scenario), Encoding.UTF8, "application/json");
        }
    }
}
