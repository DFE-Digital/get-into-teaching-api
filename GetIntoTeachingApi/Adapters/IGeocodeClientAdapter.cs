using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace GetIntoTeachingApi.Adapters
{
    public interface IGeocodeClientAdapter
    {
        Task<Point> GeocodePostcodeAsync(string postcode);
    }
}
