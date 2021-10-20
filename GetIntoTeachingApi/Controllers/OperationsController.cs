using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/operations")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        // Must be a substantial amount longer than the max expected offline duration
        // of the CRM and less than 24 hours (the point at which we auto-purge any held PII).
        private static readonly TimeSpan CrmIntegrationAutoResumeInterval = TimeSpan.FromHours(6);
        private readonly IStore _store;
        private readonly ICrmService _crm;
        private readonly INotifyService _notifyService;
        private readonly IHangfireService _hangfire;
        private readonly IRedisService _redis;
        private readonly IEnv _env;
        private readonly IAppSettings _appSettings;
        private readonly IBackgroundJobClient _jobClient;

        public OperationsController(
            ICrmService crm,
            IStore store,
            INotifyService notifyService,
            IHangfireService hangfire,
            IRedisService redis,
            IEnv env,
            IAppSettings appSettings,
            IBackgroundJobClient jobClient)
        {
            _store = store;
            _crm = crm;
            _notifyService = notifyService;
            _hangfire = hangfire;
            _redis = redis;
            _env = env;
            _appSettings = appSettings;
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

        [HttpPut]
        [Authorize(Roles = "Admin,Crm")]
        [Route("pause_crm_integration")]
        [SwaggerOperation(
            Summary = "Temporarily pauses the integration with the CRM.",
            Description = "The CRM is taken offline for updates occasionally; this can result " +
            "in errors when the API attempts to call out to the CRM. The CRM can call this endpoint " +
            "to pause the API -> CRM integration (if not manually resumed it will auto-resume in 6 hours).",
            OperationId = "PauseCrmIntegration",
            Tags = new[] { "Operations" })]
        [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status204NoContent)]
        public IActionResult PauseCrmIntegration()
        {
            _appSettings.CrmIntegrationPausedUntil = DateTime.UtcNow.AddHours(CrmIntegrationAutoResumeInterval.TotalHours);

            return NoContent();
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Crm")]
        [Route("resume_crm_integration")]
        [SwaggerOperation(
            Summary = "Resumes the integration with the CRM (after being paused).",
            OperationId = "ResumeCrmIntegration",
            Tags = new[] { "Operations" })]
        [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status204NoContent)]
        public IActionResult ResumeCrmIntegration()
        {
            _appSettings.CrmIntegrationPausedUntil = null;

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("backfill_find_apply_candidates")]
        [SwaggerOperation(
            Summary = "Triggers a backfill job to sync the CRM with the Find/Apply candidates.",
            Description = "The backfill will query all candidate information from the Find/Apply API and " +
            "queue jobs to sync the data with the CRM.",
            OperationId = "BackfillFindApplyCandidates",
            Tags = new[] { "Operations" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult BackfillFindApplyCandidates()
        {
            if (_appSettings.IsFindApplyBackfillInProgress)
            {
                return BadRequest("Backfill already in progress");
            }

            _jobClient.Enqueue<FindApplyBackfillJob>((x) => x.RunAsync());

            return NoContent();
        }
    }
}