using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/teaching_events")]
    [ApiController]
    [LogRequests]
    [Authorize]
    public class TeachingEventsController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly ICrmService _crm;
        private readonly IStore _store;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger<TeachingEventsController> _logger;

        public TeachingEventsController(
            IStore store,
            IBackgroundJobClient jobClient,
            ICandidateAccessTokenService tokenService,
            ICrmService crm,
            ILogger<TeachingEventsController> logger)
        {
            _store = store;
            _jobClient = jobClient;
            _crm = crm;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet]
        [CrmETag]
        [PrivateShortTermResponseCache]
        [Route("search_indexed_by_type")]
        [SwaggerOperation(
            Summary = "Searches for teaching events, returning grouped by type.",
            Description = @"Searches for teaching events. Optionally limit the results by distance (in miles) from a postcode, event type and start date.",
            OperationId = "SearchTeachingEventsIndexedByType",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(IDictionary<string, IEnumerable<TeachingEvent>>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SearchIndexedByType(
            [FromQuery, SwaggerParameter("Event search criteria.", Required = true)] TeachingEventSearchRequest request,
            [FromQuery, SwaggerParameter("Quantity to return (per type).")] int quantityPerType = 3)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            if (request.Postcode != null)
            {
                _logger.LogInformation($"SearchIndexedByType: {request.Postcode}");
            }

            var teachingEvents = await _store.SearchTeachingEventsAsync(request);
            return Ok(IndexTeachingEventsByType(teachingEvents, quantityPerType));
        }

        [HttpGet]
        [CrmETag]
        [PrivateShortTermResponseCache]
        [Route("upcoming_indexed_by_type")]
        [SwaggerOperation(
            Summary = "Retrieves upcoming teaching events grouped by type.",
            Description = @"Retrieves upcoming teaching events grouped by type and limited to a given quantity per type.",
            OperationId = "UpcomingTeachingEventsIndexedByType",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(IDictionary<string, IEnumerable<TeachingEvent>>), 200)]
        public IActionResult UpcomingIndexedByType([FromQuery, SwaggerParameter("Quantity to return (per type).")] int quantityPerType = 3)
        {
            var teachingEventsByType = _store.GetUpcomingTeachingEvents();
            return Ok(IndexTeachingEventsByType(teachingEventsByType, quantityPerType));
        }

        [HttpGet]
        [CrmETag]
        [PrivateShortTermResponseCache]
        [Route("{readableId}")]
        [SwaggerOperation(
            Summary = "Retrieves an event.",
            OperationId = "GetTeachingEvent",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(TeachingEvent), 200)]
        [ProducesResponseType(404)]
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
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(404)]
        public IActionResult AddAttendee(
            [FromBody, SwaggerRequestBody("Attendee to add to the teaching event.", Required = true)] TeachingEventAddAttendee request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(request.Candidate, null));

            return NoContent();
        }

        [HttpPost]
        [Route("attendees/{accessToken}")]
        [SwaggerOperation(
            Summary = "Retrieves a pre-populated TeachingEventAddAttendee for the candidate.",
            Description = @"
Retrieves a pre-populated TeachingEventAddAttendee for the candidate. The `accessToken` is obtained from a 
`POST /candidates/access_tokens` request (you must also ensure the `ExistingCandidateRequest` payload you 
exchanged for your token matches the request payload here).",
            OperationId = "GetPreFilledTeachingEventAddAttendee",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(TeachingEventAddAttendee), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetAttendee(
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

            return Ok(new TeachingEventAddAttendee(candidate));
        }

        private static IDictionary<string, IEnumerable<TeachingEvent>> IndexTeachingEventsByType(
            IEnumerable<TeachingEvent> teachingEvents,
            int quantityPerType)
        {
            return teachingEvents
                .ToList()
                .GroupBy(e => e.TypeId.ToString())
                .ToDictionary(g => g.Key, g => g.Take(quantityPerType));
        }
    }
}
