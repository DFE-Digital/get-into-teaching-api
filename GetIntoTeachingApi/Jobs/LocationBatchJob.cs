using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Database;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Location = GetIntoTeachingApi.Models.Location;

namespace GetIntoTeachingApi.Jobs
{
    public class LocationBatchJob : BaseJob
    {
        private readonly GetIntoTeachingDbContext _dbContext;

        public LocationBatchJob(GetIntoTeachingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task RunAsync(string batchJson)
        {
            var batchLocations = JsonConvert.DeserializeObject<List<ExpandoObject>>(
                    batchJson, new ExpandoObjectConverter()).Select(l => (dynamic) l).ToList();
            var batchPostcodes = batchLocations.Select(l => l.Postcode);
            var existingPostcodes = await _dbContext.Locations
                .Where(l => batchPostcodes.Contains(l.Postcode))
                .Select(l => l.Postcode)
                .ToListAsync();
            var newBatchLocations = batchLocations.Where(l => !existingPostcodes.Contains(l.Postcode));

            await _dbContext.Locations.AddRangeAsync(newBatchLocations.Select(CreateLocation));
            await _dbContext.SaveChangesAsync();
        }

        private static Location CreateLocation(dynamic location)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: DbConfiguration.Wgs84Srid);
            var coordinate = geometryFactory.CreatePoint(new Coordinate(location.Longitude, location.Latitude));

            return new Location() { Postcode = location.Postcode, Coordinate = coordinate };
        }
    }
}
