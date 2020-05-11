using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/privacy_policies")]
    [ApiController]
    [Authorize(Policy = "SharedSecret")]
    public class PrivacyPoliciesController : ControllerBase
    {
        private readonly ILogger<PrivacyPoliciesController> _logger;
        private readonly ICrmService _crm;

        public PrivacyPoliciesController(ILogger<PrivacyPoliciesController> logger, ICrmService crm)
        {
            _logger = logger;
            _crm = crm;
        }

        [HttpGet]
        [Route("latest")]
        [SwaggerOperation(
            Summary = "Retrieves the latest privacy policy.",
            OperationId = "GetLatestPrivacyPolicy",
            Tags = new[] { "Privacy Policies" }
        )]
        [ProducesResponseType(typeof(PrivacyPolicy), 200)]
        public async Task<IActionResult> GetLatest()
        {
            PrivacyPolicy privacyPolicy = await _crm.GetLatestPrivacyPolicy();
            return Ok(privacyPolicy);
        }
    }
}