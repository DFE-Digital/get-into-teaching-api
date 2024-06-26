﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                new Mock<IRedisService>().Object, DbContext, _mockLogger.Object, _metrics);
        }

        [Fact]
        public void FreeMapToolsUrl_IsCorrect()
        {
            LocationSyncJob.FreeMapToolsUrl.Should().Be("https://data.freemaptools.com/download/full-uk-postcodes/ukpostcodes.zip");
        }

        [Fact]
        public async Task RunAsync_UpsertsLocations()
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

            var csvLocations = CsvLocations();

            DbContext.Locations.Count().Should().Be(csvLocations.Count());
            DbContext.Locations.ToList().All(existingLocation => csvLocations.Any(csvLocation =>
                MatchCsvLocationWithExisting(csvLocation, existingLocation))).Should().BeTrue();
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

        [Fact]
        public async Task RunAsync_WhenCsvOmitsHeaderRow_UpsertsLocations()
        {
            _mockEnv.Setup(m => m.IsDevelopment).Returns(false);
            var server = WireMockServer.Start();
            var ukPostcodeCsvUrl = $"http://localhost:{server.Ports[0]}/test";
            server
                .Given(Request.Create().WithUrl(ukPostcodeCsvUrl))
                .RespondWith(Response.Create()
                    .WithBodyFromFile("./Fixtures/ukpostcodes_no_header_row.csv.zip")
                );

            await _job.RunAsync(ukPostcodeCsvUrl);
            // Run again to verify upsert/no duplicates
            await _job.RunAsync(ukPostcodeCsvUrl);

            var csvLocations = CsvLocations();

            DbContext.Locations.Count().Should().Be(csvLocations.Count());
            DbContext.Locations.ToList().All(existingLocation => csvLocations.Any(csvLocation =>
                MatchCsvLocationWithExisting(csvLocation, existingLocation))).Should().BeTrue();
            DbContext.Locations.All(l => l.Source == GetIntoTeachingApi.Models.Location.SourceType.CSV);
        }

        [Fact]
        public async Task RunAsync_WhenCsvContainsMalformedRows_SkipsMalformedRows()
        {
            _mockEnv.Setup(m => m.IsDevelopment).Returns(false);
            var server = WireMockServer.Start();
            var ukPostcodeCsvUrl = $"http://localhost:{server.Ports[0]}/test";
            server
                .Given(Request.Create().WithUrl(ukPostcodeCsvUrl))
                .RespondWith(Response.Create()
                    .WithBodyFromFile("./Fixtures/ukpostcodes_malformed_rows.csv.zip")
                );

            await _job.RunAsync(ukPostcodeCsvUrl);
            // Run again to verify upsert/no duplicates
            await _job.RunAsync(ukPostcodeCsvUrl);

            var csvLocations = CsvLocations();

            DbContext.Locations.Count().Should().Be(3);
            DbContext.Locations.ToList().All(existingLocation => csvLocations.Any(csvLocation =>
                MatchCsvLocationWithExisting(csvLocation, existingLocation))).Should().BeTrue();
            DbContext.Locations.All(l => l.Source == GetIntoTeachingApi.Models.Location.SourceType.CSV);
        }

        private static IEnumerable<dynamic> CsvLocations()
        {
            return new List<dynamic>
            {
                new { Postcode = "ky119yu", Latitude = 56.02748, Longitude = -3.35870 },
                new { Postcode = "ca48le", Latitude = 54.89014, Longitude = -2.84000 },
                new { Postcode = "ky62nj", Latitude = 56.182790, Longitude = -3.178240 },
                new { Postcode = "kw14yl", Latitude = 58.64102, Longitude = -3.10075 },
                new { Postcode = "tr182ab", Latitude = 50.12279, Longitude = -5.53987 },
            };
        }

        private static bool MatchCsvLocationWithExisting(dynamic csvLocation, GetIntoTeachingApi.Models.Location existingLocation)
        {
            var postcodeMatch = csvLocation.Postcode == existingLocation.Postcode;
            var csvLocationCoordinate = Coordinate(csvLocation.Latitude, csvLocation.Longitude);
            var coordinateMatch = csvLocationCoordinate == existingLocation.Coordinate;

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