﻿using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prometheus;
using Location = GetIntoTeachingApi.Models.Location;

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
                var existingPostcodes = await _dbContext.Locations
                    .Where(l => batchPostcodes.Contains(l.Postcode))
                    .Select(l => l.Postcode)
                    .ToListAsync();
                var newBatchLocations = batchLocations.Where(l => !existingPostcodes.Contains(l.Postcode));

                await _dbContext.Locations.AddRangeAsync(newBatchLocations.Select(CreateLocation));
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"LocationBatchJob - Succeeded");
            }
        }

        private static Location CreateLocation(dynamic location)
        {
            return new Location(location.Postcode, location.Latitude, location.Longitude);
        }
    }
}
