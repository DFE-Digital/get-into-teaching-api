using System.Collections.Generic;
using GetIntoTeachingApi.Jobs;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class LocationSyncJobTests
    {
        private readonly LocationSyncJob _job;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;

        public LocationSyncJobTests()
        {
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _job = new LocationSyncJob(_mockJobClient.Object);
        }

        [Fact]
        public async void RunAsync_EnqueuesLocationBatchJob()
        {
            var server = WireMockServer.Start();
            var ukPostcodeCsvUrl = $"http://localhost:{server.Ports[0]}/test";
            server
                .Given(Request.Create().WithUrl(ukPostcodeCsvUrl))
                .RespondWith(Response.Create()
                    .WithBodyFromFile("./Fixtures/ukpostcodes.csv.zip")
                );

            await _job.RunAsync(ukPostcodeCsvUrl);

            var expectedLocationBatch = new List<dynamic>
            {
                new { Postcode = "ky119yu", Latitude = 56.02748, Longitude = -3.35870 },
                new { Postcode = "ca48le", Latitude = 54.89014, Longitude = -2.84000 },
                new { Postcode = "ky62nj", Latitude = 56.182790, Longitude = -3.178240 },
                new { Postcode = "kw14yl", Latitude = 58.64102, Longitude = -3.10075 },
                new { Postcode = "tr182ab", Latitude = 50.12279, Longitude = -5.53987 },
            };

            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(LocationBatchJob) && 
                                  job.Method.Name == "RunAsync" && 
                                  (string) job.Args[0] == JsonConvert.SerializeObject(expectedLocationBatch)),
                It.IsAny<EnqueuedState>()));
        }
    }
}