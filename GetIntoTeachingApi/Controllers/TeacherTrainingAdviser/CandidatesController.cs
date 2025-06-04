using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using GetIntoTeachingApi.Models.TeacherTrainingAdviser;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Controllers.TeacherTrainingAdviser
{
    [Route("api/teacher_training_adviser/candidates")]
    [ApiController]
    [Authorize(Roles = "Admin,GetAnAdviser,Apply,GetIntoTeaching")]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly ICrmService _crm;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IDateTimeProvider _dateTime;
        private readonly IAppSettings _appSettings;
        private readonly ILogger<CandidatesController> _logger;
        private readonly IDegreeStatusDomainService _degreeStatusDomainService;
        private readonly ICurrentYearProvider _currentYearProvider;

        public CandidatesController(
            ICandidateAccessTokenService tokenService,
            ICrmService crm,
            IBackgroundJobClient jobClient,
            IDateTimeProvider dateTime,
            IAppSettings appSettings,
            ILogger<CandidatesController> logger,
            IDegreeStatusDomainService degreeStatusDomainService,
            ICurrentYearProvider currentYearProvider)
        {
            _crm = crm;
            _tokenService = tokenService;
            _jobClient = jobClient;
            _dateTime = dateTime;
            _appSettings = appSettings;
            _logger = logger;
            _degreeStatusDomainService = degreeStatusDomainService;
            _currentYearProvider = currentYearProvider;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Sign up a candidate for the Teacher Training Adviser service.",
            Description = "Queue a candidate upsert job.",
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

            // This is a short-medium term solution to infer the degree status from the
            // graduation year provided to support current behaviour until degree status
            // is fully retired. The intention being to remove this functionality once we
            // fully migrate to the new approach. 
            int? degreeStatusId =
                request.InferDegreeStatus(
                    _degreeStatusDomainService, _currentYearProvider);

            // This is the only way we can mock/freeze the current date/time
            // in contract tests (there's no other way to inject it into this class).
            request.DateTimeProvider = _dateTime;
            string json = request.Candidate.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((upsertCandidateJob) => upsertCandidateJob.Run(json, null));

            _logger.LogInformation("TeacherTrainingAdviser - CandidatesController - Sign Up - {Client}", User.Identity.Name);

            return Ok(new DegreeStatusResponse { DegreeStatusId = degreeStatusId });
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
            request.Reference ??= User.Identity.Name;

            var candidate = _crm.MatchCandidate(request);

            if (candidate == null || !_tokenService.IsValid(accessToken, request, (Guid)candidate.Id))
            {
                return Unauthorized();
            }

            return Ok(new TeacherTrainingAdviserSignUp(candidate));
        }

        [HttpPost]
        [Route("matchback")]
        [SwaggerOperation(
           Summary = "Perform a matchback operation to retrieve a pre-populated TeacherTrainingAdviserSignUp for the candidate.",
           Description = @"Attempts to matchback against a known candidate and returns a pre-populated TeacherTrainingAdviser sign up if a match is found.",
           OperationId = "MatchbackCandidate",
           Tags = new[] { "Teacher Training Adviser" })]
        [ProducesResponseType(typeof(TeacherTrainingAdviserSignUp), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public IActionResult Matchback([FromBody, SwaggerRequestBody("Candidate details to matchback.", Required = true)] ExistingCandidateRequest request)
        {
            request.Reference ??= User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_appSettings.IsCrmIntegrationPaused)
            {
                _logger.LogInformation("TeacherTrainingAdviser - CandidatesController - potential duplicate (CRM integration paused)");
                return NotFound();
            }

            Candidate candidate;

            try
            {
                candidate = _crm.MatchCandidate(request);
            }
            catch (Exception e)
            {
                _logger.LogInformation("TeacherTrainingAdviser - CandidatesController - potential duplicate (CRM exception) - {Message}", e.Message);
                throw;
            }

            if (candidate == null)
            {
                return NotFound();
            }

            return Ok(new TeacherTrainingAdviserSignUp(candidate));
        }
    }
}
