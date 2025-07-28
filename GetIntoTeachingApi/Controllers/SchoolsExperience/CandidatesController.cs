﻿using System;
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
    //[Authorize(Roles = "Admin,SchoolsExperience")]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly ICrmService _crm;
        private readonly ICandidateUpserter _upserter;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IDateTimeProvider _dateTime;

        public CandidatesController(
            ICandidateAccessTokenService tokenService,
            ICrmService crm,
            ICandidateUpserter upserter,
            IBackgroundJobClient jobClient,
            IDateTimeProvider dateTime)
        {
            _crm = crm;
            _upserter = upserter;
            _tokenService = tokenService;
            _jobClient = jobClient;
            _dateTime = dateTime;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Sign up a candidate for the Schools Experience service.",
            Description = @"
                Upsert a candidate. Returns the updated candidate information in the body of the response along 
                with a Location header which specifies the location of the candidate",
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

            // This is the only way we can mock/freeze the current date/time
            // in contract tests (there's no other way to inject it into this class).
            request.DateTimeProvider = _dateTime;

            var candidate = request.Candidate;

            if (appSettings.IsCrmIntegrationPaused)
            {
                QueueUpsert(candidate);
            }
            else
            {
                try
                {
                    _upserter.Upsert(candidate);
                }
                catch
                {
                    QueueUpsert(candidate);
                }
            }

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

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        [HttpGet]
        [Route("with-relationships")]
        [SwaggerOperation(
           Summary = "Retrieves existing candidates with relationships",
           OperationId = "GetMultipleWithRelationships",
           Tags = new[] { "Candidate with multiple relationships" })]
        [Produces("application/json")]
        //[ProducesResponseType(typeof(IEnumerable<Candidate>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMultipleWithRelationships()
        {
            var contactChannelCreations = _crm.GetAllCandidatesContactChannelCreations();
            //var signUps = candidates.Select(c => new SchoolsExperienceSignUp(c));

            return Ok(contactChannelCreations);
        }


        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="request"></param>
        /// <returns></returns>


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
            request.Reference ??= User.Identity.Name;

            var candidate = _crm.MatchCandidate(request);

            if (candidate == null || !_tokenService.IsValid(accessToken, request, (Guid)candidate.Id))
            {
                return Unauthorized();
            }

            return Ok(new SchoolsExperienceSignUp(candidate));
        }

        [HttpPost]
        [Route("{id}/school_experience")]
        [SwaggerOperation(
            Summary = "Add a school experience to the candidate.",
            Description = "Adds a new school experience to the candidate record",
            OperationId = "AddSchoolExperience",
            Tags = new[] { "Schools Experience" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public IActionResult AddSchoolExperience(
            [FromRoute, SwaggerParameter("The `id` of the `Candidate`.", Required = true)] Guid id,
            [FromBody, SwaggerRequestBody("School experience.", Required = true)] CandidateSchoolExperience candidateSchoolExperience)
        {
            var candidate = new Candidate { Id = id };
            candidate.SchoolExperiences.Add(candidateSchoolExperience);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string json = candidate.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));

            return NoContent();
        }

        private void QueueUpsert(Candidate candidate)
        {
            // Usually, it is best practice to allow the CRM to generate sequential GUIDs which provide better
            // SQL performance. However, when the CRM is unavailable we have agreed it is beneficial to
            // provide the GUID up-front because the School Experience app needs the Candidate ID immediately.
            candidate.GenerateUpfrontId();

            string json = candidate.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));
        }
    }
}
