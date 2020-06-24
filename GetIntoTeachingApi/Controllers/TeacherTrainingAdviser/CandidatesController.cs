using System.Collections.Generic;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers.TeacherTrainingAdviser
{
    [Route("api/teacher_training_adviser/candidates")]
    [ApiController]
    [Authorize]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly ICrmService _crm;
        private readonly IBackgroundJobClient _jobClient;

        public CandidatesController(
            ICandidateAccessTokenService tokenService,
            ICrmService crm,
            IBackgroundJobClient jobClient)
        {
            _crm = crm;
            _tokenService = tokenService;
            _jobClient = jobClient;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Upserts a candidate for the Teacher Training Adviser service.",
            OperationId = "UpsertTeacherTrainingAdviserCandidate",
            Tags = new[] { "Teacher Training Adviser" })]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public IActionResult Upsert(
            [FromBody, SwaggerRequestBody("Candidate to upsert for the Teacher Training Adviser service.", Required = true)] Candidate candidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            _jobClient.Enqueue<CandidateRegistrationJob>((x) => x.Run(candidate, null));

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
            Tags = new[] { "Teacher Training Adviser" })]
        [ProducesResponseType(typeof(Candidate), 200)]
        [ProducesResponseType(404)]
        public IActionResult Get(
            [FromRoute, SwaggerParameter("Access token (PIN code).", Required = true)] string accessToken,
            [FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] ExistingCandidateRequest request)
        {
            if (!_tokenService.IsValid(accessToken, request))
            {
                return Unauthorized();
            }

            var candidate = _crm.MatchCandidate(request);

            if (candidate == null)
            {
                return NotFound();
            }

            return Ok(candidate);
        }
    }
}
