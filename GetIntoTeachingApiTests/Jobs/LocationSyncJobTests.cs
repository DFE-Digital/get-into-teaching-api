using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging;
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
        private readonly Mock<ILogger<LocationSyncJob>> _mockLogger;
        private readonly Mock<IEnv> _mockEnv;
        private readonly IMetricService _metrics;

        public LocationSyncJobTests()
        {
            _mockEnv = new Mock<IEnv>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _mockLogger = new Mock<ILogger<LocationSyncJob>>();
            _metrics = new MetricService();
            _job = new LocationSyncJob(_mockEnv.Object, _mockJobClient.Object,
                _mockLogger.Object, _metrics);
        }

        [Fact]
        public async void RunAsync_EnqueuesLocationBatchJob()
        {
            _mockEnv.Setup(m => m.IsDevelopment).Returns(false);
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
                                  (string)job.Args[0] == JsonConvert.SerializeObject(expectedLocationBatch)),
                It.IsAny<EnqueuedState>()));

            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - Started");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - ZIP Downloaded");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - CSV Extracted");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - ZIP Deleted");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - Queueing 5 Locations (1 Jobs)");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - CSV Deleted");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - Succeeded");

            _metrics.LocationSyncDuration.Count.Should().BeGreaterOrEqualTo(1);
        }
    }
}