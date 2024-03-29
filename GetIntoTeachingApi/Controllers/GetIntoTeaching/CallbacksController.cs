﻿using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers.GetIntoTeaching
{
    [Route("api/get_into_teaching/callbacks")]
    [ApiController]
    [Authorize(Roles = "Admin,GetIntoTeaching")]
    public class CallbacksController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly ICrmService _crm;
        private readonly IDateTimeProvider _dateTime;
        private readonly IBackgroundJobClient _jobClient;

        public CallbacksController(
                    ICandidateAccessTokenService tokenService,
                    ICrmService crm,
                    IDateTimeProvider dateTime,
                    IBackgroundJobClient jobClient)
        {
            _crm = crm;
            _dateTime = dateTime;
            _tokenService = tokenService;
            _jobClient = jobClient;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Schedule a callback for the candidate.",
            Description = "Queues a candidate upsert job.",
            OperationId = "BookGetIntoTeachingCallback",
            Tags = new[] { "Get into Teaching" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public IActionResult Book(
            [FromBody, SwaggerRequestBody("Candidate to book a callback for.", Required = true)] GetIntoTeachingCallback request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
            Summary = "Retrieves a pre-populated GetIntoTeachingCallback for the candidate.",
            Description = @"
                        Retrieves a pre-populated GetIntoTeachingCallback for the candidate. The `accessToken` is obtained from a 
                        `POST /candidates/access_tokens` request (you must also ensure the `ExistingCandidateRequest` payload you 
                        exchanged for your token matches the request payload here).",
            OperationId = "ExchangeAccessTokenForGetIntoTeachingCallback",
            Tags = new[] { "Get into Teaching" })]
        [ProducesResponseType(typeof(GetIntoTeachingCallback), StatusCodes.Status200OK)]
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

            return Ok(new GetIntoTeachingCallback(candidate));
        }

        [HttpPost]
        [Route("matchback")]
        [SwaggerOperation(
           Summary = "Perform a matchback operation to retrieve a pre-populated GetIntoTeachingCallback for the candidate.",
           Description = @"Attempts to matchback against a known candidate and returns a pre-populated GetIntoTeachingCallback if a match is found.",
           OperationId = "MatchbackGetIntoTeachingCallback",
           Tags = new[] { "Get into Teaching" })]
        [ProducesResponseType(typeof(GetIntoTeachingCallback), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public IActionResult Matchback([FromBody, SwaggerRequestBody("Candidate details to matchback.", Required = true)] ExistingCandidateRequest request)
        {
            request.Reference ??= User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var candidate = _crm.MatchCandidate(request);

            if (candidate == null)
            {
                return NotFound();
            }

            return Ok(new GetIntoTeachingCallback(candidate));
        }
    }
}
