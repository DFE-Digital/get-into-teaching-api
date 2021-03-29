using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers.TeacherTrainingAdviser
{
    [Route("api/teacher_training_adviser/candidates")]
    [ApiController]
    [Authorize(Roles = "Admin,GetAnAdviser")]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly ICrmService _crm;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IDateTimeProvider _dateTime;

        public CandidatesController(
            ICandidateAccessTokenService tokenService,
            ICrmService crm,
            IBackgroundJobClient jobClient,
            IDateTimeProvider dateTime)
        {
            _crm = crm;
            _tokenService = tokenService;
            _jobClient = jobClient;
            _dateTime = dateTime;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Sign up a candidate for the Teacher Training Adviser service.",
            Description = "Validation errors may be present on the `TeacherTrainingAdviserSignUp` object as " +
                          "well as the hidden `Candidate` model that is mapped to; property names are " +
                          "consistent, so you should check for inclusion of the field in the key " +
                          "when linking an error message back to a property on the request model. For " +
                          "example, an error on `DegreeSubject` can return under the keys " +
                          "`Candidate.Qualifications[0].DegreeSubject` and `DegreeSubject`.",
            OperationId = "SignUpTeacherTrainingAdviserCandidate",
            Tags = new[] { "Teacher Training Adviser" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public IActionResult SignUp(
            [FromBody, SwaggerRequestBody("Candidate to sign up for the Teacher Training Adviser service.", Required = true)] TeacherTrainingAdviserSignUp request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            // This is the only way we can mock/freeze the current date/time
            // in contract tests (there's no other way to inject it into this class).
            request.DateTimeProvider = _dateTime;
            string json = request.Candidate.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));

            return NoContent();
        }

        [HttpPost]
        [Route("exchange_access_token/{accessToken}")]
        [SwaggerOperation(
            Summary = "Retrieves a pre-populated TeacherTrainingAdviserSignUp for the candidate.",
            Description = @"
                Retrieves a pre-populated TeacherTrainingAdviserSignUp for the candidate. The `accessToken` is obtained from a 
                `POST /candidates/access_tokens` request (you must also ensure the `ExistingCandidateRequest` payload you 
                exchanged for your token matches the request payload here).",
            OperationId = "ExchangeAccessTokenForTeacherTrainingAdviserSignUp",
            Tags = new[] { "Teacher Training Adviser" })]
        [ProducesResponseType(typeof(TeacherTrainingAdviserSignUp), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ExchangeAccessToken(
            [FromRoute, SwaggerParameter("Access token (PIN code).", Required = true)] string accessToken,
            [FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] ExistingCandidateRequest request)
        {
            var candidate = _crm.MatchCandidate(request);

            if (candidate == null || !_tokenService.IsValid(accessToken, request, (Guid)candidate.Id))
            {
                return Unauthorized();
            }

            return Ok(new TeacherTrainingAdviserSignUp(candidate));
        }
    }
}
