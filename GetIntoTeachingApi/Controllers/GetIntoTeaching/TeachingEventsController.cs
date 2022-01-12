using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Models.GetIntoTeaching.Validators;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers.GetIntoTeaching
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
        private readonly IDateTimeProvider _dateTime;

        public TeachingEventsController(
            IStore store,
            IBackgroundJobClient jobClient,
            ICandidateAccessTokenService tokenService,
            ICrmService crm,
            ILogger<TeachingEventsController> logger,
            IMetricService metrics,
            IDateTimeProvider dateTime)
        {
            _store = store;
            _jobClient = jobClient;
            _crm = crm;
            _metrics = metrics;
            _tokenService = tokenService;
            _logger = logger;
            _dateTime = dateTime;
        }

        [HttpGet]
        [PrivateShortTermResponseCache]
        [Route("search_grouped_by_type")]
        [SwaggerOperation(
            Summary = "Searches for teaching events, returning grouped by type.",
            Description = "Searches for teaching events. Optionally limit the results by distance (in miles) from a postcode, event type and start date.",
            OperationId = "SearchTeachingEventsGroupedByType",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(IEnumerable<TeachingEventsByType>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchGroupedByType(
            [FromQuery, CommaSeparated("TypeIds", "StatusIds"), SwaggerParameter("Event search criteria.", Required = true)] TeachingEventSearchRequest request,
            [FromQuery, SwaggerParameter("Quantity to return (per type).")] int quantityPerType = 3)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Postcode != null)
            {
                _logger.LogInformation("SearchGroupedByType: {Postcode}", request.Postcode);
            }

            var teachingEvents = await _store.SearchTeachingEventsAsync(request);

            var typeIds = request.TypeIds == null ? string.Empty : string.Join(",", request.TypeIds);

            _metrics.TeachingEventSearchResults
                .WithLabels(typeIds, request.Radius.ToString())
                .Observe(teachingEvents.Count());

            var inPesonTeachingEvents = teachingEvents.Where(e => e.IsInPerson);
            _metrics.InPersonTeachingEventResults
                .WithLabels(typeIds, request.Radius.ToString())
                .Observe(inPesonTeachingEvents.Count());

            return Ok(GroupTeachingEventsByType(teachingEvents, quantityPerType));
        }

        [HttpGet]
        [PrivateShortTermResponseCache]
        [Route("search")]
        [SwaggerOperation(
            Summary = "Searches for teaching events.",
            Description = "Searches for teaching events. Optionally limit the results by distance (in miles) from a postcode, event type and start date.",
            OperationId = "SearchTeachingEvents",
            Tags = new[] { "Teaching Events" })]
                [ProducesResponseType(typeof(IEnumerable<TeachingEvent>), StatusCodes.Status200OK)]
                [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search(
            [FromQuery, CommaSeparated("TypeIds", "StatusIds"), SwaggerParameter("Event search criteria.", Required = true)] TeachingEventSearchRequest request,
            [FromQuery, SwaggerParameter("Quantity to return.")] int quantity = 10)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Postcode != null)
            {
                _logger.LogInformation("Search: {Postcode}", request.Postcode);
            }

            var teachingEvents = (await _store.SearchTeachingEventsAsync(request)).Take(quantity);

            var typeIds = request.TypeIds == null ? string.Empty : string.Join(",", request.TypeIds);

            _metrics.TeachingEventSearchResults
                .WithLabels(typeIds, request.Radius.ToString())
                .Observe(teachingEvents.Count());

            var inPesonTeachingEvents = teachingEvents.Where(e => e.IsInPerson);
            _metrics.InPersonTeachingEventResults
                .WithLabels(typeIds, request.Radius.ToString())
                .Observe(inPesonTeachingEvents.Count());

            return Ok(teachingEvents);
        }

        [HttpGet]
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
            Description = @"
                If the `CandidateId` is specified then the existing candidate will be 
                registered for the event, otherwise a new candidate will be created.",
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
        [Route("attendees/exchange_unverified_request")]
        [SwaggerOperation(
            Summary = "Retrieves a pre-populated TeachingEventAddAttendee for the candidate (allowing to proceed as unverified).",
            Description = @"
                        Retrieves a pre-populated TeachingEventAddAttendee for the candidate. This mechanism should be used with caution
                        and the candidate should be treated as 'unverified' by the client.",
            OperationId = "ExchangeUnverifiedRequestForTeachingEventAddAttendee",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(TeachingEventAddAttendee), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ExchangeUnverifiedRequestForAttendee(
            [FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] ExistingCandidateRequest request)
        {
            request.Reference ??= User.Identity.Name;

            var candidate = _crm.MatchCandidate(request);

            if (candidate == null)
            {
                return NotFound();
            }

            var attendee = new TeachingEventAddAttendee(candidate)
            {
                IsVerified = false,
            };

            attendee.ClearAttributesForUnverifiedAccess();

            return Ok(attendee);
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
            request.Reference ??= User.Identity.Name;

            var candidate = _crm.MatchCandidate(request);

            if (candidate == null || !_tokenService.IsValid(accessToken, request, (Guid)candidate.Id))
            {
                return Unauthorized();
            }

            return Ok(new TeachingEventAddAttendee(candidate));
        }

        [HttpPost]
        [Route("")]
        [SwaggerOperation(
            Summary = "Adds or updates a teaching event.",
            Description = "If the `id` is specified then the existing teaching event will be " +
                          "updated, otherwise a new teaching event will be created.",
            OperationId = "UpsertTeachingEvent",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(TeachingEvent), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upsert(
            [FromBody, SwaggerRequestBody("Teaching event to upsert.", Required = true)] TeachingEvent teachingEvent,
            [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            ValidateForUpsertOperation(teachingEvent);

            if (!ModelState.IsValid)
            {
                return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
            }

            // Persist the building independently first so that we
            // can populate the building id on the event prior to persisting.
            await PersistBuildingAsync(teachingEvent);
            await PersistEventAsync(teachingEvent);

            return CreatedAtAction(
                actionName: nameof(Get),
                routeValues: new { readableId = teachingEvent.ReadableId },
                value: teachingEvent);
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

        private async Task PersistBuildingAsync(TeachingEvent teachingEvent)
        {
            if (teachingEvent.Building == null)
            {
                return;
            }

            _crm.Save(teachingEvent.Building);
            await _store.SaveAsync(teachingEvent.Building);
            teachingEvent.BuildingId = teachingEvent.Building.Id;
        }

        private async Task PersistEventAsync(TeachingEvent teachingEvent)
        {
            // Remove building before persisting to prevent error.
            var tempBuilding = teachingEvent.Building;
            teachingEvent.Building = null;
            _crm.Save(teachingEvent);

            // Restore building before persiting to cache.
            teachingEvent.Building = tempBuilding;
            await _store.SaveAsync(teachingEvent);
        }

        private void ValidateForUpsertOperation(TeachingEvent teachingEvent)
        {
            var operation = new TeachingEventUpsertOperation(teachingEvent);
            var validator = new TeachingEventUpsertOperationValidator(_crm);
            var result = validator.Validate(operation);

            result.AddToModelState(ModelState, null);
        }
    }
}
