using FluentAssertions;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApiTests.Helpers;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;
using Location = GetIntoTeachingApi.Models.Location;

namespace GetIntoTeachingApiTests.Jobs
{
    public class LocationSyncJobTests : DatabaseTests
    {
        private readonly LocationSyncJob _job;

        public LocationSyncJobTests()
        {
            _job = new LocationSyncJob(DbContext);
        }

        [Fact]
        public async void RunAsync_InsertsNewLocations()
        {
            var server = WireMockServer.Start();
            var ukPostcodeCsvUrl = $"http://localhost:{server.Ports[0]}/test";
            server
                .Given(Request.Create().WithUrl(ukPostcodeCsvUrl))
                .RespondWith(Response.Create()
                    .WithBodyFromFile("./Fixtures/ukpostcodes.csv.zip")
                );
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: DbConfiguration.Wgs84Srid);
            var existingLocation = new Location()
                {Postcode = "ky119yu", Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.35870, 56.02748))};
            await DbContext.Locations.AddAsync(existingLocation);
            await DbContext.SaveChangesAsync();

            await _job.RunAsync(ukPostcodeCsvUrl);

            var expectedLocations = new Location[]
            {
                existingLocation,
                new Location() {Postcode = "ca48le", Coordinate = geometryFactory.CreatePoint(new Coordinate(-2.84000, 54.89014))},
                new Location() {Postcode = "ky62nj", Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.178240, 56.182790))},
                new Location() {Postcode = "kw14yl", Coordinate = geometryFactory.CreatePoint(new Coordinate(-3.10075, 58.64102))},
                new Location() {Postcode = "tr182ab", Coordinate = geometryFactory.CreatePoint(new Coordinate(-5.53987, 50.12279))},
            };

            DbContext.Locations.Should().BeEquivalentTo(expectedLocations);
        }
    }
}