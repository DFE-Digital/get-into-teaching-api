using System.Threading.Tasks;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/privacy_policies")]
    [ApiController]
    [Authorize(Policy = "SharedSecret")]
    public class PrivacyPoliciesController : ControllerBase
    {
        private readonly ILogger<PrivacyPoliciesController> _logger;
        private readonly IWebApiClient _client;

        public PrivacyPoliciesController(ILogger<PrivacyPoliciesController> logger, IWebApiClient client)
        {
            _logger = logger;
            _client = client;
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
            var privacyPolicy = await _client.GetLatestPrivacyPolicy();
            return Ok(privacyPolicy);
        }
    }
}