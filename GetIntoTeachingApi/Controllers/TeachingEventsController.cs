using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/teaching_events")]
    [ApiController]
    [Authorize(Policy = "SharedSecret")]
    public class TeachingEventsController : ControllerBase
    {
        private const int MaximumUpcomingRequests = 50;
        private readonly ILogger<TeachingEventsController> _logger;
        private readonly ICrmService _crm;

        public TeachingEventsController(ILogger<TeachingEventsController> logger, ICrmService crm)
        {
            _logger = logger;
            _crm = crm;
        }

        [HttpGet]
        [Route("upcoming")]
        [SwaggerOperation(
            Summary = "Retrieves the upcoming teaching events.",
            Description = @"
Retrieves the upcoming teaching events; limited to 10 by default, but this can be increased to a 
maximum of 50 using the `limit` query parameter.",
            OperationId = "GetUpcomingTeachingEvents",
            Tags = new[] { "Teaching Events" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TeachingEvent>), 200)]
        [ProducesResponseType(400)]
        public IActionResult GetUpcoming([FromQuery, SwaggerParameter("Number of results to return (maximum of 50).")] int limit = 10)
        {
            if (limit > MaximumUpcomingRequests)
                return BadRequest();

            var upcomingEvents = _crm.GetUpcomingTeachingEvents(limit);
            return Ok(upcomingEvents);
        }

        [HttpGet]
        [Route("search")]
        [SwaggerOperation(
            Summary = "Searches for teaching events.",
            Description = @"Searches for teaching events by postcode. Optionally limit the results by distance (in miles) and the type of event.",
            OperationId = "SearchTeachingEvents",
            Tags = new[] { "Teaching Events" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TeachingEvent>), 200)]
        [ProducesResponseType(400)]
        public IActionResult Search([FromQuery, SwaggerParameter("Event search criteria.", Required = true)] TeachingEventSearchRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(this.ModelState);

            var teachingEvents = _crm.SearchTeachingEvents(request);
            return Ok(teachingEvents);
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Retrieves an event.",
            OperationId = "GetTeachingEvent",
            Tags = new[] { "Teaching Events" }
        )]
        [ProducesResponseType(typeof(TeachingEvent), 200)]
        [ProducesResponseType(404)]
        public IActionResult Get([FromRoute, SwaggerParameter("The `id` of the `TeachingEvent`.", Required = true)] Guid id)
        {
            var teachingEvent = _crm.GetTeachingEvent(id);

            if (teachingEvent == null)
                return NotFound();

            return Ok(teachingEvent);
        }

        [HttpPost]
        [Route("{id}/attendees")]
        [SwaggerOperation(
            Summary = "Adds an attendee to a teaching event.",
            OperationId = "AddTeachingEventAttendee",
            Tags = new[] { "Teaching Events" }
        )]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(404)]
        public IActionResult AddAttendee(
            [FromRoute, SwaggerParameter("The `id` of the `TeachingEvent`.", Required = true)] Guid id,
            [FromBody, SwaggerRequestBody("Attendee to add to the teaching event.", Required = true)] ExistingCandidateRequest attendee
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(this.ModelState);

            var teachingEvent = _crm.GetTeachingEvent(id);
            var candidate = _crm.GetCandidate(attendee);

            if (teachingEvent == null || candidate == null)
                return NotFound();

            var registration = new TeachingEventRegistration()
            {
                CandidateId = (Guid) candidate.Id, 
                EventId = (Guid) teachingEvent.Id
            };

            _crm.Save(registration);

            return NoContent();
        }
    }
}
