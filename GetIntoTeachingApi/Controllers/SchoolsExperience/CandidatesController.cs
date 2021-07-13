using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.SchoolsExperience;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers.SchoolsExperience
{
    [Route("api/schools_experience/candidates")]
    [ApiController]
    [Authorize(Roles = "Admin,SchoolsExperience")]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly ICrmService _crm;
        private readonly ICandidateUpserter _upserter;
        private readonly IBackgroundJobClient _jobClient;

        public CandidatesController(
            ICandidateAccessTokenService tokenService,
            ICrmService crm,
            ICandidateUpserter upserter,
            IBackgroundJobClient jobClient)
        {
            _crm = crm;
            _upserter = upserter;
            _tokenService = tokenService;
            _jobClient = jobClient;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Sign up a candidate for the Schools Experience service.",
            Description = "Validation errors may be present on the `SchoolsExperienceSignUp` object as " +
                          "well as the hidden `Candidate` model that is mapped to; property names are " +
                          "consistent, so you should check for inclusion of the field in the key " +
                          "when linking an error message back to a property on the request model. For " +
                          "example, an error on `DegreeSubject` can return under the keys " +
                          "`Candidate.Qualifications[0].DegreeSubject` and `DegreeSubject`.",
            OperationId = "SignUpSchoolsExperienceCandidate",
            Tags = new[] { "Schools Experience" })]
        [ProducesResponseType(typeof(SchoolsExperienceSignUp), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public IActionResult SignUp(
            [FromBody, SwaggerRequestBody("Candidate to sign up for the Schools Experience service.", Required = true)] SchoolsExperienceSignUp request,
            [FromServices] IAppSettings appSettings)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (appSettings.IsCrmIntegrationPaused)
            {
                throw new InvalidOperationException("CandidatesController#SignUp - Aborting (CRM integration paused).");
            }

            var candidate = request.Candidate;
            _upserter.Upsert(candidate);

            return CreatedAtAction(
                actionName: nameof(Get),
                routeValues: new { id = candidate.Id },
                value: new SchoolsExperienceSignUp(candidate));
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Retrieves an existing SchoolsExperienceSignUp for the candidate.",
            OperationId = "GetSchoolsExperienceSignUp",
            Tags = new[] { "Schools Experience" })]
        [ProducesResponseType(typeof(SchoolsExperienceSignUp), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get([FromRoute, SwaggerParameter("The `id` of the `Candidate`.", Required = true)] Guid id)
        {
            var candidate = _crm.GetCandidate(id);

            if (candidate == null)
            {
                return NotFound();
            }

            return Ok(new SchoolsExperienceSignUp(candidate));
        }

        [HttpGet]
        [Route("")]
        [SwaggerOperation(
            Summary = "Retrieves existing SchoolsExperienceSignUps for the candidate `ids`.",
            OperationId = "GetSchoolsExperienceSignUps",
            Tags = new[] { "Schools Experience" })]
        [ProducesResponseType(typeof(IEnumerable<SchoolsExperienceSignUp>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMultiple([FromQuery, CommaSeparated, SwaggerParameter("A collection of `Candidate` `id`s.", Required = true)] IEnumerable<Guid> ids)
        {
            var candidates = _crm.GetCandidates(ids);
            var signUps = candidates.Select(c => new SchoolsExperienceSignUp(c));

            return Ok(signUps);
        }

        [HttpPost]
        [Route("exchange_access_token/{accessToken}")]
        [SwaggerOperation(
            Summary = "Retrieves a pre-populated SchoolsExperienceSignUp for the candidate.",
            Description = @"
                Retrieves a pre-populated SchoolsExperienceSignUp for the candidate. The `accessToken` is obtained from a 
                `POST /candidates/access_tokens` request (you must also ensure the `ExistingCandidateRequest` payload you 
                exchanged for your token matches the request payload here).",
            OperationId = "ExchangeAccessTokenForSchoolsExperienceSignUp",
            Tags = new[] { "Schools Experience" })]
        [ProducesResponseType(typeof(SchoolsExperienceSignUp), StatusCodes.Status200OK)]
        public IActionResult ExchangeAccessToken(
            [FromRoute, SwaggerParameter("Access token (PIN code).", Required = true)] string accessToken,
            [FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] ExistingCandidateRequest request)
        {
            var candidate = _crm.MatchCandidate(request);

            if (candidate == null || !_tokenService.IsValid(accessToken, request, (Guid)candidate.Id))
            {
                return Unauthorized();
            }

            return Ok(new SchoolsExperienceSignUp(candidate));
        }

        [HttpPost]
        [Route("{id}/classroom_experience_notes")]
        [SwaggerOperation(
           Summary = "Add a classroom experience note to the candidate.",
           Description = @"Adds a new classroom experience note to the candidate record",
           OperationId = "AddClassroomExperienceNote",
           Tags = new[] { "Schools Experience" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AddClassroomExperienceNote(
           [FromRoute, SwaggerParameter("The `id` of the `Candidate`.", Required = true)] Guid id,
           [FromBody, SwaggerRequestBody("Classroom experience note.", Required = true)] ClassroomExperienceNote note)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCandidate = _crm.GetCandidate(id);

            if (existingCandidate == null)
            {
                return NotFound();
            }

            // Create a new candidate to encapsulate the actual changes - avoids writing
            // all the existingCandidate fields back to the CRM.
            var candidate = new Candidate()
            {
                Id = id,
                ClassroomExperienceNotesRaw = existingCandidate.ClassroomExperienceNotesRaw,
            };

            candidate.AddClassroomExperienceNote(note);

            string json = candidate.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));

            return NoContent();
        }
    }
}
