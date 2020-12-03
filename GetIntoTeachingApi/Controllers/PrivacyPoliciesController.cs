using System;
using System.Threading.Tasks;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/privacy_policies")]
    [ApiController]
    [LogRequests]
    [PrivateShortTermResponseCache]
    [Authorize]
    public class PrivacyPoliciesController : ControllerBase
    {
        private readonly IStore _store;

        public PrivacyPoliciesController(IStore store)
        {
            _store = store;
        }

        [HttpGet]
        [CrmETag]
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

        [HttpGet]
        [CrmETag]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Retrieves a privacy policy.",
            OperationId = "GetPrivacyPolicy",
            Tags = new[] { "Privacy Policies" })]
        [ProducesResponseType(typeof(PrivacyPolicy), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromRoute, SwaggerParameter("The `id` of the `PrivacyPolicy`.", Required = true)] Guid id)
        {
            var policy = await _store.GetPrivacyPolicyAsync(id);

            if (policy == null)
            {
                return NotFound();
            }

            return Ok(policy);
        }
    }
}