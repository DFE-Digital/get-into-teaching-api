using GetIntoTeachingApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/candidates")]
    [ApiController]
    [Authorize(Policy = "SharedSecret")]
    public class CandidatesController : ControllerBase
    {
        private readonly ILogger<CandidatesController> _logger;

        public CandidatesController(ILogger<CandidatesController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("access_token")]
        [SwaggerOperation(
            Summary = "Creates a candidate access token.",
            Description = @"
Finds a candidate matching at least 3 of the provided CandidateAccessTokenRequest attributes. 
If a candidate is found, an access token (PIN code) will be sent to the candidate email address 
that can then be used for verification.",
            OperationId = "CreateCandidateAccessToken",
            Tags = new[] { "Candidates" }
        )]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public IActionResult CreateAccessToken([FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] CandidateAccessTokenRequest candidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            // TODO:
            return NoContent();
        }
    }
}
