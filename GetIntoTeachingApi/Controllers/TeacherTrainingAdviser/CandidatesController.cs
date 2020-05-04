using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Controllers.TeacherTrainingAdviser
{
    [Route("api/teacher_training_adviser/candidates")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly ILogger<CandidatesController> _logger;
        private readonly ICandidateAccessTokenService _tokenService;

        public CandidatesController(ILogger<CandidatesController> logger, ICandidateAccessTokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Upserts a candidate for the Teacher Training Adviser service.",
            OperationId = "UpsertTeacherTrainingAdviserCandidate",
            Tags = new[] { "Teacher Training Adviser" }
        )]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public IActionResult Upsert([FromBody, SwaggerRequestBody("Candidate to upsert for the Teacher Training Adviser service.", Required = true)] Object candidate) // TODO:
        {
            // TODO:
            return Ok(new Object());
        }

        [HttpGet]
        [Route("{accessToken}")]
        [SwaggerOperation(
            Summary = "Retrieves an existing candidate for the Teacher Training Adviser service.",
            Description = @"
Retrieves an existing candidate for the Teacher Training Adviser service. The `accessToken` is obtained from a 
`POST /candidates/access_tokens` request and must be sent along with the shared secret in the `Authorization` header.",
            OperationId = "GetExistingTeacherTrainingAdviserCandidate",
            Tags = new[] { "Teacher Training Adviser" }
        )]
        [ProducesResponseType(401)]
        public IActionResult Get(
            [FromRoute, SwaggerParameter("Access token (PIN code).", Required = true)] string accessToken,
            [FromHeader(Name = "Candidate-Email"), SwaggerParameter("Candidate email address.", Required = true)] string email
        )
        {
            var challenge = new CandidateAccessTokenChallenge { Token = accessToken, Email = email };
            if (!_tokenService.IsValid(challenge))
            {
                return Unauthorized();
            }

            // TODO:
            return Ok(new Object());
        }
    }
}
