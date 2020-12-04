using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prometheus;

namespace GetIntoTeachingApi.Jobs
{
    public class LocationBatchJob : BaseJob
    {
        private readonly GetIntoTeachingDbContext _dbContext;
        private readonly ILogger<LocationBatchJob> _logger;
        private readonly IMetricService _metrics;

        public LocationBatchJob(
            IEnv env,
            GetIntoTeachingDbContext dbContext,
            ILogger<LocationBatchJob> logger,
            IMetricService metrics)
            : base(env)
        {
            _dbContext = dbContext;
            _logger = logger;
            _metrics = metrics;
        }

        public async Task RunAsync(string batchJson)
        {
            using (_metrics.LocationBatchDuration.NewTimer())
            {
                _logger.LogInformation($"LocationBatchJob - Started");

                var batchLocations = JsonConvert.DeserializeObject<List<ExpandoObject>>(
                    batchJson, new ExpandoObjectConverter()).Select(l => (dynamic)l).ToList();
                var batchPostcodes = batchLocations.Select(l => l.Postcode);
                var existingLocations = _dbContext.Locations
                    .Where(l => batchPostcodes.Contains(l.Postcode));
                var existingPostcodes = await existingLocations
                    .Select(l => l.Postcode).ToListAsync();
                var newBatchLocations = batchLocations.Where(l => !existingPostcodes.Contains(l.Postcode));

                CorrectUnknownSources(existingLocations);

                await _dbContext.Locations.AddRangeAsync(newBatchLocations.Select(CreateLocation));
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"LocationBatchJob - Succeeded");
            }
        }

        private static Location CreateLocation(dynamic location)
        {
            return new Location(location.Postcode, location.Latitude, location.Longitude, Source.CSV);
        }

        private void CorrectUnknownSources(IQueryable<Location> locations)
        {
            foreach (var location in locations)
            {
                if (location.Source == Source.Unknown)
                {
                    location.Source = Source.CSV;
                }
            }
        }
    }
}
