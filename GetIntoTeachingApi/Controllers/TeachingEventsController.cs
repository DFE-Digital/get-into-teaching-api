using System.Collections.Generic;
using System.Threading.Tasks;
using GetIntoTeachingApi.Filters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/teaching_events")]
    [ApiController]
    [Authorize]
    public class TeachingEventsController : ControllerBase
    {
        private const int MaximumUpcomingRequests = 50;
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly ICrmService _crm;
        private readonly IStore _store;
        private readonly IBackgroundJobClient _jobClient;

        public TeachingEventsController(
            IStore store,
            IBackgroundJobClient jobClient,
            ICandidateAccessTokenService tokenService,
            ICrmService crm)
        {
            _store = store;
            _jobClient = jobClient;
            _crm = crm;
            _tokenService = tokenService;
        }

        [HttpGet]
        [CrmETag]
        [Route("upcoming")]
        [SwaggerOperation(
            Summary = "Retrieves the upcoming teaching events.",
            Description = @"
Retrieves the upcoming teaching events; limited to 10 by default, but this can be increased to a 
maximum of 50 using the `limit` query parameter.",
            OperationId = "GetUpcomingTeachingEvents",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(IEnumerable<TeachingEvent>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetUpcoming([FromQuery, SwaggerParameter("Number of results to return (maximum of 50).")] int limit = 10)
        {
            if (limit > MaximumUpcomingRequests)
            {
                return BadRequest();
            }

            var upcomingEvents = _store.GetUpcomingTeachingEvents(limit);
            return Ok(await upcomingEvents.ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("search")]
        [SwaggerOperation(
            Summary = "Searches for teaching events.",
            Description = @"Searches for teaching events by postcode. Optionally limit the results by distance (in miles) and the type of event.",
            OperationId = "SearchTeachingEvents",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(IEnumerable<TeachingEvent>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Search([FromQuery, SwaggerParameter("Event search criteria.", Required = true)] TeachingEventSearchRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            var teachingEvents = await _store.SearchTeachingEventsAsync(request);
            return Ok(teachingEvents);
        }

        [HttpGet]
        [CrmETag]
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
    }
}
