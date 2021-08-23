using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    [Collection("Database")]
    public class LocationSyncJobTests : DatabaseTests
    {
        private readonly LocationSyncJob _job;
        private readonly Mock<ILogger<LocationSyncJob>> _mockLogger;
        private readonly Mock<IEnv> _mockEnv;
        private readonly IMetricService _metrics;

        public LocationSyncJobTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            _mockEnv = new Mock<IEnv>();
            _mockLogger = new Mock<ILogger<LocationSyncJob>>();
            _metrics = new MetricService();
            _job = new LocationSyncJob(_mockEnv.Object,
                DbContext, _mockLogger.Object, _metrics);
        }

        [Fact]
        public void FreeMapToolsUrl_IsCorrect()
        {
            LocationSyncJob.FreeMapToolsUrl.Should().Be("https://www.freemaptools.com/download/full-uk-postcodes/ukpostcodes.zip");
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
            // Run again to verify upsert/no duplicates
            await _job.RunAsync(ukPostcodeCsvUrl);

            var batch = new List<dynamic>
            {
                new { Postcode = "ky119yu", Latitude = 56.02748, Longitude = -3.35870 },
                new { Postcode = "ca48le", Latitude = 54.89014, Longitude = -2.84000 },
                new { Postcode = "ky62nj", Latitude = 56.182790, Longitude = -3.178240 },
                new { Postcode = "kw14yl", Latitude = 58.64102, Longitude = -3.10075 },
                new { Postcode = "tr182ab", Latitude = 50.12279, Longitude = -5.53987 },
            };

            DbContext.Locations.Count().Should().Be(batch.Count);
            DbContext.Locations.ToList().All(l =>
                batch.Any(b => BatchLocationMatchesExistingLocation(b, l))).Should().BeTrue();
            DbContext.Locations.All(l => l.Source == GetIntoTeachingApi.Models.Location.SourceType.CSV);

            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - Started");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - ZIP Downloaded");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - CSV Extracted");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - ZIP Deleted");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - Processed 5 Locations (1 Batches)");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - CSV Deleted");
            _mockLogger.VerifyInformationWasCalled("LocationSyncJob - Succeeded");

            _metrics.LocationSyncDuration.Count.Should().BeGreaterOrEqualTo(1);
        }

        private static bool BatchLocationMatchesExistingLocation(dynamic batchLocation, GetIntoTeachingApi.Models.Location existingLocation)
        {
            var postcodeMatch = batchLocation.Postcode == existingLocation.Postcode;
            var batchCoordinate = Coordinate(batchLocation.Latitude, batchLocation.Longitude);
            var coordinateMatch = batchCoordinate == existingLocation.Coordinate;

            return postcodeMatch && coordinateMatch;
        }

        private static Point Coordinate(double latitude, double longitude)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: DbConfiguration.Wgs84Srid);
            var coordinate = new Coordinate(longitude, latitude);

            return geometryFactory.CreatePoint(coordinate);
        }
    }
}