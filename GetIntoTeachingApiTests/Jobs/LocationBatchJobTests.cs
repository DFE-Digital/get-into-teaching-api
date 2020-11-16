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
using Newtonsoft.Json;
using Xunit;
using Location = GetIntoTeachingApi.Models.Location;

namespace GetIntoTeachingApiTests.Jobs
{
    [Collection("Database")]
    public class LocationBatchJobTests : DatabaseTests
    {
        private readonly LocationBatchJob _job;
        private readonly IMetricService _metrics;
        private readonly Mock<ILogger<LocationBatchJob>> _mockLogger;

        public LocationBatchJobTests(DatabaseFixture databaseFixture): base(databaseFixture)
        {
            _mockLogger = new Mock<ILogger<LocationBatchJob>>();
            _metrics = new MetricService();
            _job = new LocationBatchJob(new Env(), DbContext, _mockLogger.Object, _metrics);
        }

        [Fact]
        public async void RunAsync_InsertsNewLocations()
        {
            var batch = new List<dynamic>
            {
                new { Postcode = "ky119yu", Latitude = 56.02748, Longitude = -3.35870 },
                new { Postcode = "ca48le", Latitude = 54.89014, Longitude = -2.84000 },
                new { Postcode = "ky62nj", Latitude = 56.182790, Longitude = -3.178240 },
                new { Postcode = "kw14yl", Latitude = 58.64102, Longitude = -3.10075 },
                new { Postcode = "tr182ab", Latitude = 50.12279, Longitude = -5.53987 },
            };

            await _job.RunAsync(JsonConvert.SerializeObject(batch));
            await _job.RunAsync(JsonConvert.SerializeObject(batch));

            DbContext.Locations.Count().Should().Be(batch.Count());
            DbContext.Locations.ToList().All(l =>
                batch.Any(b => BatchLocationMatchesExistingLocation(b, l))).Should().BeTrue();

            _mockLogger.VerifyInformationWasCalled("LocationBatchJob - Started");
            _mockLogger.VerifyInformationWasCalled("LocationBatchJob - Succeeded");

            _metrics.LocationBatchDuration.Count.Should().Be(2);
        }

        private static bool BatchLocationMatchesExistingLocation(dynamic batchLocation, Location existingLocation)
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