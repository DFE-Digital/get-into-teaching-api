using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/operations")]
    [ApiController]
    [LogRequests]
    public class OperationsController : ControllerBase
    {
        private readonly IStore _store;
        private readonly ICrmService _crm;
        private readonly INotifyService _notifyService;
        private readonly IHangfireService _hangfire;
        private readonly IEnv _env;
        private readonly IBackgroundJobClient _jobClient;

        public OperationsController(
            ICrmService crm,
            IStore store,
            INotifyService notifyService,
            IHangfireService hangfire,
            IEnv env,
            IBackgroundJobClient jobClient)
        {
            _store = store;
            _crm = crm;
            _notifyService = notifyService;
            _hangfire = hangfire;
            _env = env;
            _jobClient = jobClient;
        }

        [HttpGet]
        [Route("generate_mapping_info")]
        [SwaggerOperation(
            Summary = "Generates the mapping information.",
            Description = "Generates the mapping information describing how the " +
                          "models in the API map to the corresponding entities in Dynamics 365.",
            OperationId = "GenerateMappingInfo",
            Tags = new[] { "Operations" })]
        [ProducesResponseType(typeof(IEnumerable<MappingInfo>), 200)]
        public IActionResult GenerateMappingInfo()
        {
            var assembly = typeof(BaseModel).Assembly;
            var subTypes = assembly.GetTypes().Where(t => t.BaseType == typeof(BaseModel));
            var mappings = subTypes.Select(s => new MappingInfo(s));

            return Ok(mappings.ToList());
        }

        [HttpGet]
        [Route("health_check")]
        [SwaggerOperation(
            Summary = "Performs a health check.",
            OperationId = "HealthCheck",
            Tags = new[] { "Operations" })]
        [ProducesResponseType(typeof(HealthCheckResponse), 200)]
        public async Task<IActionResult> HealthCheck()
        {
            var response = new HealthCheckResponse()
            {
                GitCommitSha = _env.GitCommitSha,
                Environment = _env.EnvironmentName,
                Hangfire = _hangfire.CheckStatus(),
                Database = await _store.CheckStatusAsync(),
                Crm = _crm.CheckStatus(),
                Notify = await _notifyService.CheckStatusAsync(),
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("simulate_error")]
        [SwaggerOperation(
            Summary = "Simulates a 500 error to test the Sentry integration.",
            OperationId = "SimulateError",
            Tags = new[] { "Operations" })]
        [ProducesResponseType(typeof(HealthCheckResponse), 200)]
        public void SimulateError()
        {
            System.Text.StringBuilder builder = null;

            builder.Append("throw error");
        }

        [HttpGet]
        [Authorize]
        [Route("trigger_location_sync")]
        [SwaggerOperation(
            Summary = "Manually triggers a location sync job",
            OperationId = "TriggerLocationSync",
            Tags = new[] { "Operations" })]
        [ProducesResponseType(typeof(void), 200)]
        public void TriggerLocationSync()
        {
            _jobClient.Enqueue<LocationSyncJob>(job => job.RunAsync(LocationSyncJob.FreeMapToolsUrl));
        }

        [HttpGet]
        [Authorize]
        [Route("remove_unknown_locations")]
        [SwaggerOperation(
      Summary = "Remove locations of unknown source from the database",
      OperationId = "RemoveUnknownLocations",
      Tags = new[] { "Operations" })]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> RemoveUnknownLocations(bool dryRun)
        {
            if (dryRun)
            {
                return Ok($"Number of locations with Unknown source: {_store.GetNumberOfUnknownLocations()}");
            }

            await _store.RemoveUnknownLocations();
            return Ok();
        }
    }
}