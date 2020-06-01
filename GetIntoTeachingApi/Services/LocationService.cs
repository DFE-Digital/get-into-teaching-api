using System.Linq;
using System.Text.RegularExpressions;
using GetIntoTeachingApi.Database;

namespace GetIntoTeachingApi.Services
{
    public class LocationService : ILocationService
    { 
        private const double MetersToMiles = 0.000621371;
        private readonly GetIntoTeachingDbContext _dbContext;

        public LocationService() { }

        public LocationService(GetIntoTeachingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsValid(string postcode)
        {
            if (string.IsNullOrEmpty(postcode))
                return false;

            return _dbContext.Locations.Any(l => l.Postcode == Sanitize(postcode));
        }

        public double DistanceBetween(string originPostcode, string destinationPostcode)
        {
            var postcodes = new[] {Sanitize(originPostcode), Sanitize(destinationPostcode)};
            var locations = _dbContext.Locations.Where(l => postcodes.Contains(l.Postcode)).ToList();

            return locations.First().Coordinate.GetDistanceTo(locations.Last().Coordinate) * MetersToMiles;
        }

        public static string Sanitize(string postcode)
        {
            return Regex.Replace(postcode, @"\s+", "").ToLower();
        }
    }
}
