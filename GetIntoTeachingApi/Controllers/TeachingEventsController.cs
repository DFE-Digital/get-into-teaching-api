using System;
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
        private readonly IStore _store;
        private readonly IBackgroundJobClient _jobClient;

        public TeachingEventsController(IStore store, IBackgroundJobClient jobClient)
        {
            _store = store;
            _jobClient = jobClient;
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
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Retrieves an event.",
            OperationId = "GetTeachingEvent",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(TeachingEvent), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromRoute, SwaggerParameter("The `id` of the `TeachingEvent`.", Required = true)] Guid id)
        {
            var teachingEvent = await _store.GetTeachingEventAsync(id);

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
                          "registered for the event, otherwise a new candidate will be created.",
            OperationId = "AddTeachingEventAttendee",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(404)]
        public IActionResult AddAttendee(
            [FromBody, SwaggerRequestBody("Attendee to add to the teaching event.", Required = true)] TeachingEventAddAttendeeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(request.Candidate, null));

            return NoContent();
        }
    }
}
