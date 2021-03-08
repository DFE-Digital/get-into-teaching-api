using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/operations")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IStore _store;
        private readonly ICrmService _crm;
        private readonly INotifyService _notifyService;
        private readonly IHangfireService _hangfire;
        private readonly IRedisService _redis;
        private readonly IEnv _env;

        public OperationsController(
            ICrmService crm,
            IStore store,
            INotifyService notifyService,
            IHangfireService hangfire,
            IRedisService redis,
            IEnv env)
        {
            _store = store;
            _crm = crm;
            _notifyService = notifyService;
            _hangfire = hangfire;
            _redis = redis;
            _env = env;
        }

        [HttpGet]
        [Route("generate_mapping_info")]
        [SwaggerOperation(
            Summary = "Generates the mapping information.",
            Description = "Generates the mapping information describing how the " +
                          "models in the API map to the corresponding entities in Dynamics 365.",
            OperationId = "GenerateMappingInfo",
            Tags = new[] { "Operations" })]
        [ProducesResponseType(typeof(IEnumerable<MappingInfo>), StatusCodes.Status200OK)]
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
        [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
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
                Redis = await _redis.CheckStatusAsync(),
            };

            return Ok(response);
        }
    }
}