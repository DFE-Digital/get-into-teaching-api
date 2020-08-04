using System.Linq;
using System.Threading.Tasks;
using GeocodeSharp.Google;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using NetTopologySuite.Geometries;

namespace GetIntoTeachingApi.Adapters
{
    public class GeocodeClientAdapter : IGeocodeClientAdapter
    {
        private readonly IMetricService _metrics;
        private readonly GeocodeClient _client;

        public GeocodeClientAdapter(IEnv env, IMetricService metrics)
        {
            _metrics = metrics;
            _client = new GeocodeClient(env.GoogleApiKey);
        }

        public async Task<Point> GeocodePostcodeAsync(string postcode)
        {
            _metrics.GoogleApiCalls.Inc();

            var response = await _client.GeocodeAddress(postcode);

            if (response.Status != GeocodeStatus.Ok)
            {
                return null;
            }

            var result = response.Results.FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            var location = result.Geometry.Location;
            return new Point(new Coordinate(location.Longitude, location.Latitude));
        }
    }
}
