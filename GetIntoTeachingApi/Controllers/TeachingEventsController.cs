﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/teaching_events")]
    [ApiController]
    [Authorize(Roles = "Admin,GetIntoTeaching")]
    public class TeachingEventsController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly ICrmService _crm;
        private readonly IStore _store;
        private readonly IMetricService _metrics;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger<TeachingEventsController> _logger;

        public TeachingEventsController(
            IStore store,
            IBackgroundJobClient jobClient,
            ICandidateAccessTokenService tokenService,
            ICrmService crm,
            ILogger<TeachingEventsController> logger,
            IMetricService metrics)
        {
            _store = store;
            _jobClient = jobClient;
            _crm = crm;
            _metrics = metrics;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet]
        [CrmETag]
        [PrivateShortTermResponseCache]
        [Route("search_grouped_by_type")]
        [SwaggerOperation(
    Summary = "Searches for teaching events, returning grouped by type.",
    Description = @"Searches for teaching events. Optionally limit the results by distance (in miles) from a postcode, event type and start date.",
    OperationId = "SearchTeachingEventsGroupedByType",
    Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(IEnumerable<TeachingEventsByType>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchGroupedByType(
    [FromQuery, SwaggerParameter("Event search criteria.", Required = true)] TeachingEventSearchRequest request,
    [FromQuery, SwaggerParameter("Quantity to return (per type).")] int quantityPerType = 3)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            if (request.Postcode != null)
            {
                _logger.LogInformation($"SearchGroupedByType: {request.Postcode}");
            }

            var teachingEvents = await _store.SearchTeachingEventsAsync(request);

            _metrics.TeachingEventSearchResults.WithLabels(request.TypeId.ToString(), request.Radius.ToString()).Observe(teachingEvents.Count());

            return Ok(GroupTeachingEventsByType(teachingEvents, quantityPerType));
        }

        [HttpGet]
        [CrmETag]
        [PrivateShortTermResponseCache]
        [Route("{readableId}")]
        [SwaggerOperation(
            Summary = "Retrieves an event.",
            OperationId = "GetTeachingEvent",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(TeachingEvent), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute, SwaggerParameter("The `readableId` of the `TeachingEvent`.", Required = true)] string readableId)
        {
            var teachingEvent = await _store.GetTeachingEventAsync(readableId);

            if (teachingEvent == null)
            {
                return NotFound();
            }

            return Ok(teachingEvent);
        }

        [HttpPost]
        [Route("attendees")]
        [SwaggerOperation(
            Summary = "Adds an attendee to a teaching event.",
            Description = "If the `CandidateId` is specified then the existing candidate will be " +
                          "registered for the event, otherwise a new candidate will be created." +
                          "\n\n" +
                          "Validation errors may be present on the `TeachingEventAddAttendee` object as " +
                          "well as the hidden `Candidate` model that is mapped to; property names are " +
                          "consistent, so you should check for inclusion of the field in the key " +
                          "when linking an error message back to a property on the request model. For " +
                          "example, an error on `AcceptedPolicyId` can return under the keys " +
                          "`Candidate.PrivacyPolicy.AcceptedPolicyId` and `AcceptedPolicyId`.",
            OperationId = "AddTeachingEventAttendee",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AddAttendee(
            [FromBody, SwaggerRequestBody("Attendee to add to the teaching event.", Required = true)] TeachingEventAddAttendee request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            string json = request.Candidate.SerializeChangedTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));

            return NoContent();
        }

        [HttpPost]
        [Route("attendees/exchange_access_token/{accessToken}")]
        [SwaggerOperation(
            Summary = "Retrieves a pre-populated TeachingEventAddAttendee for the candidate.",
            Description = @"
                Retrieves a pre-populated TeachingEventAddAttendee for the candidate. The `accessToken` is obtained from a 
                `POST /candidates/access_tokens` request (you must also ensure the `ExistingCandidateRequest` payload you 
                exchanged for your token matches the request payload here).",
            OperationId = "ExchangeAccessTokenForTeachingEventAddAttendee",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(TeachingEventAddAttendee), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ExchangeAccessTokenForAttendee(
            [FromRoute, SwaggerParameter("Access token (PIN code).", Required = true)] string accessToken,
            [FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] ExistingCandidateRequest request)
        {
            var candidate = _crm.MatchCandidate(request);

            if (candidate == null || !_tokenService.IsValid(accessToken, request, (Guid)candidate.Id))
            {
                return Unauthorized();
            }

            return Ok(new TeachingEventAddAttendee(candidate));
        }

        private static IEnumerable<TeachingEventsByType> GroupTeachingEventsByType(
            IEnumerable<TeachingEvent> teachingEvents,
            int quantityPerType)
        {
            return teachingEvents
                .GroupBy(e => e.TypeId, e => e, (typeId, events) => new TeachingEventsByType()
                {
                    TypeId = typeId,
                    TeachingEvents = events.Take(quantityPerType),
                });
        }
    }
}
