using System.Linq;
using System.Threading.Tasks;
using GeocodeSharp.Google;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace GetIntoTeachingApi.Adapters
{
    public class GeocodeClientAdapter : IGeocodeClientAdapter
    {
        private readonly IMetricService _metrics;
        private readonly GeocodeClient _client;
        private readonly ILogger<IGeocodeClientAdapter> _logger;

        public GeocodeClientAdapter(IEnv env, IMetricService metrics, ILogger<IGeocodeClientAdapter> logger)
        {
            _metrics = metrics;
            _client = new GeocodeClient(env.GoogleApiKey);
            _logger = logger;
        }

        public async Task<Point> GeocodePostcodeAsync(string postcode)
        {
            var response = await _client.GeocodeAddress($"postcode {postcode}");

            _logger.LogInformation("Google API Status [{Postcode}]: {StatusText}", postcode, response.StatusText);

            if (response.Status != GeocodeStatus.Ok)
            {
                _metrics.GoogleApiCalls.WithLabels("error").Inc();
                return null;
            }

            var result = response.Results.FirstOrDefault();

            if (result == null)
            {
                _metrics.GoogleApiCalls.WithLabels("fail").Inc();
                return null;
            }

            _metrics.GoogleApiCalls.WithLabels("success").Inc();

            var location = result.Geometry.Location;
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: DbConfiguration.Wgs84Srid);

            return geometryFactory.CreatePoint(new Coordinate(location.Longitude, location.Latitude));
        }
    }
}
