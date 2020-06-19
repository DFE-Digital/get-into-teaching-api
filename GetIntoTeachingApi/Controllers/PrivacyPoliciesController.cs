using System.Threading.Tasks;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/privacy_policies")]
    [ApiController]
    [Authorize]
    public class PrivacyPoliciesController : ControllerBase
    {
        private readonly IStore _store;

        public PrivacyPoliciesController(IStore store)
        {
            _store = store;
        }

        [HttpGet]
        [Route("latest")]
        [SwaggerOperation(
            Summary = "Retrieves the latest privacy policy.",
            OperationId = "GetLatestPrivacyPolicy",
            Tags = new[] { "Privacy Policies" })]
        [ProducesResponseType(typeof(PrivacyPolicy), 200)]
        public async Task<IActionResult> GetLatest()
        {
            var privacyPolicy = await _store.GetLatestPrivacyPolicyAsync();
            return Ok(privacyPolicy);
        }
    }
}