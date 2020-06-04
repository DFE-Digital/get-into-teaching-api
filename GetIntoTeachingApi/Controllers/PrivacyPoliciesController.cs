using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
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
        private readonly IStore _store;

        public PrivacyPoliciesController(ILogger<PrivacyPoliciesController> logger, IStore store)
        {
            _logger = logger;
            _store = store;
        }

        [HttpGet]
        [Route("latest")]
        [SwaggerOperation(
            Summary = "Retrieves the latest privacy policy.",
            OperationId = "GetLatestPrivacyPolicy",
            Tags = new[] { "Privacy Policies" }
        )]
        [ProducesResponseType(typeof(PrivacyPolicy), 200)]
        public IActionResult GetLatest()
        {
            var privacyPolicy = _store.GetLatestPrivacyPolicy();
            return Ok(privacyPolicy);
        }
    }
}