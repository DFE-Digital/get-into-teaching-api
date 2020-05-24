using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services.Crm;
using Microsoft.AspNetCore.Authorization;

namespace GetIntoTeachingApi.Controllers.TeacherTrainingAdviser
{
    [Route("api/teacher_training_adviser/candidates")]
    [ApiController]
    [Authorize(Policy = "SharedSecret")]
    public class CandidatesController : ControllerBase
    {
        private readonly ILogger<CandidatesController> _logger;
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly IWebApiClient _client;
        private readonly ICrmService _crm;

        public CandidatesController(
            ILogger<CandidatesController> logger, 
            ICandidateAccessTokenService tokenService,
            ICrmService crm,
            IWebApiClient client
        )
        {
            _logger = logger;
            _crm = crm;
            _client = client;
            _tokenService = tokenService;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Upserts a candidate for the Teacher Training Adviser service.",
            OperationId = "UpsertTeacherTrainingAdviserCandidate",
            Tags = new[] { "Teacher Training Adviser" }
        )]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public IActionResult Upsert(
            [FromBody, SwaggerRequestBody("Candidate to upsert for the Teacher Training Adviser service.", Required = true)] Candidate candidate
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(this.ModelState);

            _crm.Save(candidate);

            return NoContent();
        }

        [HttpPost]
        [Route("{accessToken}")]
        [SwaggerOperation(
            Summary = "Retrieves an existing candidate for the Teacher Training Adviser service.",
            Description = @"
Retrieves an existing candidate for the Teacher Training Adviser service. The `accessToken` is obtained from a 
`POST /candidates/access_tokens` request (you must also ensure the `ExistingCandidateRequest` payload you 
exchanged for your token matches the request payload here).",
            OperationId = "GetExistingTeacherTrainingAdviserCandidate",
            Tags = new[] { "Teacher Training Adviser" }
        )]
        [ProducesResponseType(typeof(Candidate), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(
            [FromRoute, SwaggerParameter("Access token (PIN code).", Required = true)] string accessToken, 
            [FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] ExistingCandidateRequest request
        )
        { 
            if (!_tokenService.IsValid(accessToken, request))
                return Unauthorized();

            var candidate = await _client.GetCandidate(request);

            if (candidate == null)
                return NotFound();

            return Ok(candidate);
        }
    }
}
